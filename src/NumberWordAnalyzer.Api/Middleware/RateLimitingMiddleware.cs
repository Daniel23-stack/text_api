using System.Collections.Concurrent;

namespace NumberWordAnalyzer.Api.Middleware;

public class RateLimitingMiddleware
{
    private readonly RequestDelegate _next;
    private static readonly ConcurrentDictionary<string, Queue<DateTime>> _requestHistory = new();
    private const int DefaultRequestsPerMinute = 100;

    public RateLimitingMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        var configuration = context.RequestServices.GetRequiredService<IConfiguration>();
        var requestsPerMinute = configuration.GetValue<int>("RateLimiting:RequestsPerMinute", DefaultRequestsPerMinute);

        var clientIp = context.Connection.RemoteIpAddress?.ToString() ?? "unknown";
        var now = DateTime.UtcNow;
        var oneMinuteAgo = now.AddMinutes(-1);

        var requestQueue = _requestHistory.GetOrAdd(clientIp, _ => new Queue<DateTime>());

        bool rateLimitExceeded;
        lock (requestQueue)
        {
            // Remove old requests outside the 1-minute window
            while (requestQueue.Count > 0 && requestQueue.Peek() < oneMinuteAgo)
            {
                requestQueue.Dequeue();
            }

            // Check if limit exceeded
            rateLimitExceeded = requestQueue.Count >= requestsPerMinute;
            
            if (!rateLimitExceeded)
            {
                // Record this request
                requestQueue.Enqueue(now);
            }
        }

        if (rateLimitExceeded)
        {
            context.Response.StatusCode = 429;
            context.Response.ContentType = "application/json";
            await context.Response.WriteAsync("{\"error\":\"Rate limit exceeded. Please try again later.\"}");
            return;
        }

        await _next(context);
    }
}

