# Number Word Analyzer API

[![GitHub Repository](https://img.shields.io/badge/GitHub-Repository-blue)](https://github.com/Daniel23-stack/text_api)

**Repository**: [https://github.com/Daniel23-stack/text_api](https://github.com/Daniel23-stack/text_api)

A RESTful API built with ASP.NET Core 8+ that detects and counts occurrences of English number words (one through nine) within randomized/scrambled text strings using subsequence matching algorithms.

## Features

- **Clean Architecture**: Separation of concerns with Domain, Application, Infrastructure, and API layers
- **CQRS Pattern**: Implemented using MediatR for command/query separation
- **Subsequence Matching Algorithm**: Efficiently counts number word occurrences in scrambled text
- **Comprehensive Validation**: FluentValidation for input validation
- **Error Handling**: Global error handling middleware
- **Rate Limiting**: Configurable rate limiting middleware
- **ELK Stack Integration**: Logging to Elasticsearch, Logstash, and Kibana
- **Swagger Documentation**: Auto-generated API documentation
- **Docker Support**: Containerized application with ELK stack
- **Unit Tests**: Comprehensive test coverage using xUnit, Moq, and FluentAssertions

## Architecture

The solution follows Clean Architecture principles with the following structure:

```
text_API/
├── src/
│   ├── NumberWordAnalyzer.Api/          # Controllers, middleware, startup
│   ├── NumberWordAnalyzer.Application/   # Commands, Queries, Handlers (MediatR)
│   ├── NumberWordAnalyzer.Domain/       # Domain models, interfaces
│   └── NumberWordAnalyzer.Infrastructure/ # Implementations, logging
├── tests/
│   └── NumberWordAnalyzer.Tests/        # Unit tests (xUnit)
├── docker/
│   ├── Dockerfile
│   └── docker-compose.yml               # API + ELK stack
└── NumberWordAnalyzer.sln
```

### Layers

- **Domain**: Core business logic, interfaces, and domain models
- **Application**: MediatR commands/queries, handlers, and validation
- **Infrastructure**: Service implementations (algorithm, logging)
- **API**: Controllers, middleware, and configuration

## Algorithm

The API uses a **subsequence matching algorithm** implemented with dynamic programming. For each number word (e.g., "one"), it counts how many times the letters appear in order within the input string, allowing letter reuse.

**Example**: 
- Input: "oonnee"
- For word "one": The letters o, n, e appear in order multiple times
- Result: 4 occurrences (o-o-n-n-e-e combinations)

The algorithm has a time complexity of O(n*m) where:
- n = length of input string
- m = length of the number word

## Prerequisites

- .NET 8.0 SDK or later
- Docker and Docker Compose (for containerized deployment)
- Visual Studio 2022, VS Code, or Rider (for development)

## Getting Started

### Prerequisites Check

Before starting, ensure you have the following installed:

```bash
# Check .NET SDK version (should be 8.0 or later)
dotnet --version

# Check Docker (for containerized deployment)
docker --version
docker-compose --version
```

### Local Development

#### Step 1: Clone the Repository

```bash
git clone https://github.com/Daniel23-stack/text_api.git
cd text_API
```

#### Step 2: Restore Dependencies

```bash
dotnet restore
```

This will download all required NuGet packages for all projects in the solution.

#### Step 3: Build the Solution

```bash
dotnet build
```

You should see "Build succeeded" with 0 errors and 0 warnings.

#### Step 4: Run Tests (Optional but Recommended)

```bash
dotnet test
```

All 18 tests should pass. This verifies the application is working correctly.

#### Step 5: Run the API Locally

```bash
cd src/NumberWordAnalyzer.Api
dotnet run
```

**Expected Output:**
```
Now listening on: http://localhost:5055
Now listening on: https://localhost:7084
Application started. Press Ctrl+C to shut down.
```

#### Step 6: Access the API

Once the API is running, you can access:

- **Swagger UI (Recommended)**: 
  - HTTP: `http://localhost:5055/swagger`
  - HTTPS: `https://localhost:7084/swagger`
- **API Endpoint**: `http://localhost:5055/api/NumberWordAnalyzer`

**Note**: If you see certificate warnings in the browser, click "Advanced" and "Proceed anyway" for local development.

### Docker Deployment (Recommended for Full ELK Stack)

#### Step 1: Navigate to Docker Directory

```bash
cd docker
```

#### Step 2: Build and Start All Services

```bash
docker-compose up -d
```

This command will:
- Build the API Docker image
- Download and start Elasticsearch, Kibana, and Logstash containers
- Set up networking between all services

**Expected Output:**
```
Creating network docker_elk-network
Creating volume docker_elasticsearch_data
Creating container elasticsearch
Creating container kibana
Creating container logstash
Creating container numberword-analyzer-api
Starting elasticsearch...
Starting kibana...
Starting logstash...
Starting numberword-analyzer-api...
```

**Note**: The first time you run this, it may take 2-5 minutes to download Docker images.

#### Step 3: Verify Services are Running

```bash
docker ps
```

You should see 4 containers running:
- `numberword-analyzer-api` (port 5000)
- `elasticsearch` (port 9200)
- `kibana` (port 5601)
- `logstash` (port 5044)

#### Step 4: Access the Services

Once all containers are running (wait 1-2 minutes for Kibana to fully start):

- **API & Swagger**: `http://localhost:5000/swagger`
- **Elasticsearch**: `http://localhost:9200`
- **Kibana Dashboard**: `http://localhost:5601`

#### Step 5: Set Up Kibana Dashboard (First Time Only)

1. **Open Kibana**: Navigate to `http://localhost:5601` in your browser
2. **Skip Welcome Screen**: Click "Explore on my own" or close the welcome popup
3. **Create Index Pattern**:
   - Click the hamburger menu (☰) in the top left
   - Go to **Stack Management** → **Index Patterns**
   - Click **"Create index pattern"**
   - Enter: `numberword-analyzer-logs-*`
   - Click **"Next step"**
   - Select **Time field**: `@timestamp` (should be auto-selected)
   - Click **"Create index pattern"**
4. **View Logs**:
   - Click **"Discover"** in the left sidebar
   - You should now see API logs with timestamps, log levels, and messages

#### Step 6: Test the API

Make a test request to generate logs:

```powershell
# PowerShell
$body = @{ inputText = "eeehffeetsrtiiueuefxxeexeseeetoionneghtvvsentniheinungeiefev" } | ConvertTo-Json
Invoke-RestMethod -Uri "http://localhost:5000/api/NumberWordAnalyzer" -Method Post -Body $body -ContentType "application/json"
```

```bash
# cURL
curl -X POST "http://localhost:5000/api/NumberWordAnalyzer" \
  -H "Content-Type: application/json" \
  -d '{"inputText": "eeehffeetsrtiiueuefxxeexeseeetoionneghtvvsentniheinungeiefev"}'
```

Then refresh Kibana Discover to see the new log entries!

#### Step 7: Stop Services (When Done)

```bash
cd docker
docker-compose down
```

To also remove volumes (clears Elasticsearch data):
```bash
docker-compose down -v
```

## API Usage

### Endpoint

**POST** `/api/NumberWordAnalyzer`

### Request

```json
{
  "inputText": "eeehffeetsrtiiueuefxxeexeseeetoionneghtvvsentniheinungeiefev..."
}
```

### Response

```json
{
  "wordCounts": {
    "one": 12,
    "two": 7,
    "three": 10,
    "four": 4,
    "five": 6,
    "six": 3,
    "seven": 8,
    "eight": 2,
    "nine": 5
  }
}
```

### Example using cURL

**For Local Development (port 5055):**
```bash
curl -X POST "http://localhost:5055/api/NumberWordAnalyzer" \
  -H "Content-Type: application/json" \
  -d '{"inputText": "eeehffeetsrtiiueuefxxeexeseeetoionneghtvvsentniheinungeiefev"}'
```

**For Docker (port 5000):**
```bash
curl -X POST "http://localhost:5000/api/NumberWordAnalyzer" \
  -H "Content-Type: application/json" \
  -d '{"inputText": "eeehffeetsrtiiueuefxxeexeseeetoionneghtvvsentniheinungeiefev"}'
```

### Example using PowerShell

**For Local Development (port 5055):**
```powershell
$body = @{
    inputText = "eeehffeetsrtiiueuefxxeexeseeetoionneghtvvsentniheinungeiefev"
} | ConvertTo-Json

Invoke-RestMethod -Uri "http://localhost:5055/api/NumberWordAnalyzer" `
    -Method Post `
    -Body $body `
    -ContentType "application/json"
```

**For Docker (port 5000):**
```powershell
$body = @{
    inputText = "eeehffeetsrtiiueuefxxeexeseeetoionneghtvvsentniheinungeiefev"
} | ConvertTo-Json

Invoke-RestMethod -Uri "http://localhost:5000/api/NumberWordAnalyzer" `
    -Method Post `
    -Body $body `
    -ContentType "application/json"
```

### Example using Swagger UI (Easiest Method)

1. Open `http://localhost:5000/swagger` (Docker) or `http://localhost:5055/swagger` (Local)
2. Find the `POST /api/NumberWordAnalyzer` endpoint
3. Click "Try it out"
4. Enter your input text in the request body:
   ```json
   {
     "inputText": "eeehffeetsrtiiueuefxxeexeseeetoionneghtvvsentniheinungeiefev"
   }
   ```
5. Click "Execute"
6. View the response with word counts

## Configuration

### appsettings.json

```json
{
  "Elasticsearch": {
    "Url": "http://localhost:9200"
  },
  "RateLimiting": {
    "RequestsPerMinute": 100
  }
}
```

### Environment Variables

- `ASPNETCORE_ENVIRONMENT`: Set to `Production`, `Development`, or `Staging`
- `Elasticsearch__Url`: Elasticsearch connection URL
- `RateLimiting__RequestsPerMinute`: Rate limit per IP address

## Testing

Run all tests:
```bash
dotnet test
```

Run tests with coverage:
```bash
dotnet test /p:CollectCoverage=true
```

### Test Coverage

- **Service Tests**: Algorithm correctness, edge cases
- **Handler Tests**: MediatR command handling
- **Validator Tests**: Input validation rules
- **Controller Tests**: API endpoint behavior

## Rate Limiting

The API implements rate limiting to prevent abuse:
- Default: 100 requests per minute per IP address
- Configurable via `appsettings.json`
- Returns HTTP 429 (Too Many Requests) when exceeded

## Logging

All requests and responses are logged to:
- **Console**: For local development
- **Elasticsearch**: For production (via ELK stack)
- **Structured Logging**: Using Serilog with JSON format

### Log Levels

- **Information**: Normal operations, request/response logging
- **Warning**: Non-critical issues
- **Error**: Exceptions and errors

## Error Handling

The API includes global error handling middleware that:
- Catches all unhandled exceptions
- Returns appropriate HTTP status codes
- Logs errors for debugging
- Provides user-friendly error messages

## Performance Considerations

- Algorithm optimized for large inputs using dynamic programming
- Efficient memory usage with O(n*m) space complexity
- Supports concurrent requests
- Rate limiting prevents resource exhaustion

## Technologies Used

- **ASP.NET Core 8.0**: Web API framework
- **MediatR**: CQRS pattern implementation
- **FluentValidation**: Input validation
- **Serilog**: Structured logging
- **Elasticsearch**: Log storage and search
- **Kibana**: Log visualization
- **Logstash**: Log processing
- **xUnit**: Unit testing framework
- **Moq**: Mocking framework
- **FluentAssertions**: Test assertions
- **Swashbuckle**: Swagger/OpenAPI documentation

## Project Structure

```
src/
├── NumberWordAnalyzer.Api/
│   ├── Controllers/
│   ├── Middleware/
│   ├── Models/
│   └── Program.cs
├── NumberWordAnalyzer.Application/
│   ├── Commands/
│   ├── Handlers/
│   ├── Validators/
│   └── Behaviors/
├── NumberWordAnalyzer.Domain/
│   ├── Models/
│   ├── Services/
│   └── Constants/
└── NumberWordAnalyzer.Infrastructure/
    └── Services/
```

## Quick Reference

### Common Commands

**Local Development:**
```bash
# Run API locally
cd src/NumberWordAnalyzer.Api
dotnet run

# Run tests
dotnet test

# Build solution
dotnet build
```

**Docker:**
```bash
# Start all services
cd docker
docker-compose up -d

# View logs
docker logs numberword-analyzer-api -f
docker logs elasticsearch -f
docker logs kibana -f

# Stop all services
docker-compose down

# Rebuild and restart
docker-compose build api
docker-compose up -d
```

**Access Points:**
- Local API: `http://localhost:5055/swagger`
- Docker API: `http://localhost:5000/swagger`
- Elasticsearch: `http://localhost:9200`
- Kibana: `http://localhost:5601`

## Contributing

1. Fork the repository: [https://github.com/Daniel23-stack/text_api](https://github.com/Daniel23-stack/text_api)
2. Create a feature branch: `git checkout -b feature/your-feature-name`
3. Make your changes
4. Add/update tests
5. Ensure all tests pass: `dotnet test`
6. Commit your changes: `git commit -m "Add your feature"`
7. Push to your fork: `git push origin feature/your-feature-name`
8. Submit a pull request on GitHub

## Repository

**GitHub**: [https://github.com/Daniel23-stack/text_api](https://github.com/Daniel23-stack/text_api)

Clone the repository:
```bash
git clone https://github.com/Daniel23-stack/text_api.git
cd text_API
```

## License

This project is provided as-is for evaluation purposes.

## Troubleshooting

### Port Already in Use

**Error**: "Address already in use" or "Port is already allocated"

**Solution**:
```bash
# Find process using the port (Windows PowerShell)
Get-NetTCPConnection -LocalPort 5000,5055,7084 -ErrorAction SilentlyContinue | Select-Object LocalPort, OwningProcess

# Kill the process (replace PID with actual process ID)
Stop-Process -Id <PID> -Force

# Or stop Docker containers
cd docker
docker-compose down
```

### Elasticsearch Connection Issues

**Problem**: API can't connect to Elasticsearch

**Solutions**:
1. Check if Elasticsearch is running:
   ```bash
   docker ps | grep elasticsearch
   ```
2. Check Elasticsearch health:
   ```bash
   curl http://localhost:9200/_cluster/health
   ```
3. Verify the URL in `appsettings.json` matches Docker network
4. Restart Elasticsearch:
   ```bash
   cd docker
   docker-compose restart elasticsearch
   ```

### Kibana Not Loading

**Problem**: Kibana shows "Kibana server is not ready yet"

**Solution**:
- Wait 2-3 minutes for Kibana to fully start (check logs: `docker logs kibana`)
- Ensure Elasticsearch is healthy first
- Check Kibana logs for errors: `docker logs kibana --tail 50`

### Swagger Not Loading

**Problem**: "Failed to load API definition" or 404 error

**Solutions**:
1. Ensure the API is running (check console output)
2. Try accessing: `http://localhost:5000/swagger/v1/swagger.json` directly
3. Check API logs: `docker logs numberword-analyzer-api`
4. Rebuild the Docker image:
   ```bash
   cd docker
   docker-compose build api
   docker-compose up -d api
   ```

### Rate Limiting Issues

**Problem**: Getting HTTP 429 (Too Many Requests)

**Solutions**:
1. Wait 1 minute for the rate limit window to reset
2. Adjust rate limit in `appsettings.json`:
   ```json
   {
     "RateLimiting": {
       "RequestsPerMinute": 200
     }
   }
   ```
3. Restart the API to apply changes

### Build Issues

**Problem**: Solution won't build

**Solutions**:
1. Check .NET SDK version:
   ```bash
   dotnet --version  # Should be 8.0 or later
   ```
2. Clean and restore:
   ```bash
   dotnet clean
   dotnet restore
   dotnet build
   ```
3. Delete `bin` and `obj` folders:
   ```bash
   Get-ChildItem -Path . -Include bin,obj -Recurse -Directory | Remove-Item -Recurse -Force
   dotnet restore
   dotnet build
   ```

### Docker Build Fails

**Problem**: Docker build errors

**Solutions**:
1. Ensure Docker is running: `docker ps`
2. Check Dockerfile path is correct
3. Clean Docker cache:
   ```bash
   docker system prune -a
   ```
4. Rebuild from scratch:
   ```bash
   cd docker
   docker-compose build --no-cache api
   ```

### No Logs in Kibana

**Problem**: Kibana shows no logs even after API calls

**Solutions**:
1. Verify index pattern is created: `numberword-analyzer-logs-*`
2. Check time range in Discover (top right corner)
3. Verify logs are being sent:
   ```bash
   curl http://localhost:9200/_cat/indices?v | grep numberword
   ```
4. Make a test API call and wait 10 seconds, then refresh Kibana

## Future Enhancements

- [ ] Add caching layer (Redis) for frequently analyzed texts
- [ ] Implement authentication and authorization
- [ ] Add metrics and monitoring (Prometheus/Grafana)
- [ ] Support for additional number words (ten, eleven, etc.)
- [ ] Batch processing endpoint
- [ ] WebSocket support for real-time analysis

