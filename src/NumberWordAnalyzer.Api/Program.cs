using FluentValidation;
using MediatR;
using NumberWordAnalyzer.Application.Validators;
using NumberWordAnalyzer.Domain.Services;
using NumberWordAnalyzer.Infrastructure.Services;
using Serilog;
using Serilog.Sinks.Elasticsearch;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);

// Configure Serilog for ELK Stack
var elasticsearchUrl = builder.Configuration["Elasticsearch:Url"] ?? "http://localhost:9200";
var environment = builder.Environment.EnvironmentName ?? "Development";

Log.Logger = new LoggerConfiguration()
    .Enrich.FromLogContext()
    .Enrich.WithProperty("Environment", environment)
    .WriteTo.Console()
    .WriteTo.Elasticsearch(new ElasticsearchSinkOptions(new Uri(elasticsearchUrl))
    {
        AutoRegisterTemplate = true,
        IndexFormat = $"numberword-analyzer-logs-{environment.ToLower()}-{DateTime.UtcNow:yyyy-MM}",
        NumberOfShards = 2,
        NumberOfReplicas = 1
    })
    .CreateLogger();

builder.Host.UseSerilog();

// Add services to the container
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Register MediatR
builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly()));
builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(NumberWordAnalyzer.Application.Commands.AnalyzeNumberWordsCommand).Assembly));

// Register FluentValidation
builder.Services.AddValidatorsFromAssemblyContaining<AnalyzeNumberWordsCommandValidator>();

// Register MediatR pipeline behaviors
builder.Services.AddTransient(typeof(IPipelineBehavior<,>), typeof(NumberWordAnalyzer.Application.Behaviors.ValidationBehavior<,>));

// Register domain services
builder.Services.AddScoped<INumberWordAnalyzer, NumberWordAnalyzerService>();

// Add CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline
app.UseSerilogRequestLogging();

// Enable Swagger in all environments for API documentation
app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Number Word Analyzer API v1");
    c.RoutePrefix = "swagger";
});

app.UseHttpsRedirection();

app.UseCors("AllowAll");

// Add middleware
app.UseMiddleware<NumberWordAnalyzer.Api.Middleware.ErrorHandlingMiddleware>();
app.UseMiddleware<NumberWordAnalyzer.Api.Middleware.RateLimitingMiddleware>();

app.MapControllers();

app.Run();
