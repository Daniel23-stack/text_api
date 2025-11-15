using FluentAssertions;
using Moq;
using NumberWordAnalyzer.Application.Commands;
using NumberWordAnalyzer.Application.Handlers;
using NumberWordAnalyzer.Domain.Models;
using NumberWordAnalyzer.Domain.Services;
using Xunit;

namespace NumberWordAnalyzer.Tests.Handlers;

public class AnalyzeNumberWordsCommandHandlerTests
{
    [Fact]
    public async Task Handle_WithValidInput_CallsAnalyzer()
    {
        // Arrange
        var mockAnalyzer = new Mock<INumberWordAnalyzer>();
        var expectedResult = new AnalysisResult();
        expectedResult.WordCounts["one"] = 5;

        mockAnalyzer.Setup(x => x.Analyze(It.IsAny<string>()))
            .Returns(expectedResult);

        var handler = new AnalyzeNumberWordsCommandHandler(mockAnalyzer.Object);
        var command = new AnalyzeNumberWordsCommand { InputText = "test" };

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.WordCounts["one"].Should().Be(5);
        mockAnalyzer.Verify(x => x.Analyze("test"), Times.Once);
    }

    [Fact]
    public async Task Handle_WithEmptyInput_ReturnsResult()
    {
        // Arrange
        var mockAnalyzer = new Mock<INumberWordAnalyzer>();
        var expectedResult = new AnalysisResult();

        mockAnalyzer.Setup(x => x.Analyze(It.IsAny<string>()))
            .Returns(expectedResult);

        var handler = new AnalyzeNumberWordsCommandHandler(mockAnalyzer.Object);
        var command = new AnalyzeNumberWordsCommand { InputText = string.Empty };

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        mockAnalyzer.Verify(x => x.Analyze(string.Empty), Times.Once);
    }
}

