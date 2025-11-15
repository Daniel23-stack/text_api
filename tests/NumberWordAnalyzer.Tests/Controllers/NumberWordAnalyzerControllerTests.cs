using FluentAssertions;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using NumberWordAnalyzer.Api.Controllers;
using NumberWordAnalyzer.Api.Models;
using NumberWordAnalyzer.Application.Commands;
using NumberWordAnalyzer.Domain.Models;
using Xunit;

namespace NumberWordAnalyzer.Tests.Controllers;

public class NumberWordAnalyzerControllerTests
{
    private readonly Mock<IMediator> _mockMediator;
    private readonly Mock<ILogger<NumberWordAnalyzerController>> _mockLogger;
    private readonly NumberWordAnalyzerController _controller;

    public NumberWordAnalyzerControllerTests()
    {
        _mockMediator = new Mock<IMediator>();
        _mockLogger = new Mock<ILogger<NumberWordAnalyzerController>>();
        _controller = new NumberWordAnalyzerController(_mockMediator.Object, _mockLogger.Object);
    }

    [Fact]
    public async Task Analyze_WithValidRequest_ReturnsOkResult()
    {
        // Arrange
        var request = new AnalysisRequest { InputText = "test" };
        var expectedResult = new AnalysisResult();
        expectedResult.WordCounts["one"] = 1;

        _mockMediator.Setup(x => x.Send(It.IsAny<AnalyzeNumberWordsCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedResult);

        // Act
        var result = await _controller.Analyze(request);

        // Assert
        result.Should().NotBeNull();
        var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
        var response = okResult.Value.Should().BeOfType<AnalysisResponse>().Subject;
        response.WordCounts["one"].Should().Be(1);
    }

    [Fact]
    public async Task Analyze_WithNullInputText_ReturnsOkResult()
    {
        // Arrange
        var request = new AnalysisRequest { InputText = null };
        var expectedResult = new AnalysisResult();

        _mockMediator.Setup(x => x.Send(It.IsAny<AnalyzeNumberWordsCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedResult);

        // Act
        var result = await _controller.Analyze(request);

        // Assert
        result.Should().NotBeNull();
        result.Result.Should().BeOfType<OkObjectResult>();
    }
}

