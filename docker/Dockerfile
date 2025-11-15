# Build stage
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy project files for dependency restoration
COPY ["src/NumberWordAnalyzer.Domain/NumberWordAnalyzer.Domain.csproj", "src/NumberWordAnalyzer.Domain/"]
COPY ["src/NumberWordAnalyzer.Application/NumberWordAnalyzer.Application.csproj", "src/NumberWordAnalyzer.Application/"]
COPY ["src/NumberWordAnalyzer.Infrastructure/NumberWordAnalyzer.Infrastructure.csproj", "src/NumberWordAnalyzer.Infrastructure/"]
COPY ["src/NumberWordAnalyzer.Api/NumberWordAnalyzer.Api.csproj", "src/NumberWordAnalyzer.Api/"]

# Restore dependencies for the API project (which will restore all dependencies)
WORKDIR "/src/src/NumberWordAnalyzer.Api"
RUN dotnet restore "NumberWordAnalyzer.Api.csproj"

# Copy all source files 
COPY src/ src/

# Build the API project 
RUN dotnet build "NumberWordAnalyzer.Api.csproj" -c Release -o /app/build

# Publish stage
FROM build AS publish
RUN dotnet publish "NumberWordAnalyzer.Api.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

# Copy published files
COPY --from=publish /app/publish .

# Set environment variables
ENV ASPNETCORE_URLS=http://+:8080
ENV ASPNETCORE_ENVIRONMENT=Production

ENTRYPOINT ["dotnet", "NumberWordAnalyzer.Api.dll"]

