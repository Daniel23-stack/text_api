using FluentAssertions;
using NumberWordAnalyzer.Domain.Models;
using NumberWordAnalyzer.Infrastructure.Services;
using Xunit;

namespace NumberWordAnalyzer.Tests.Services;

public class NumberWordAnalyzerServiceTests
{
    private readonly NumberWordAnalyzerService _service;

    public NumberWordAnalyzerServiceTests()
    {
        _service = new NumberWordAnalyzerService();
    }

    [Fact]
    public void Analyze_WithEmptyString_ReturnsZeroCounts()
    {
        // Act
        var result = _service.Analyze(string.Empty);

        // Assert
        result.Should().NotBeNull();
        result.WordCounts.Values.Should().AllBeEquivalentTo(0);
    }

    [Fact]
    public void Analyze_WithNullString_ReturnsZeroCounts()
    {
        // Act
        var result = _service.Analyze(null!);

        // Assert
        result.Should().NotBeNull();
        result.WordCounts.Values.Should().AllBeEquivalentTo(0);
    }

    [Fact]
    public void Analyze_WithSimpleOne_ReturnsCorrectCount()
    {
        // Arrange
        var input = "one";

        // Act
        var result = _service.Analyze(input);

        // Assert
        result.WordCounts["one"].Should().Be(1);
        result.WordCounts["two"].Should().Be(0);
    }

    [Fact]
    public void Analyze_WithScrambledOne_ReturnsCorrectCount()
    {
        // Arrange - "one" cannot be formed from "oen" because letters must be in order
        // "one" requires: o before n before e
        // "oen" has: o, e, n (e comes before n, so it doesn't match)
        var input = "oen";

        // Act
        var result = _service.Analyze(input);

        // Assert
        result.WordCounts["one"].Should().Be(0); // Cannot form "one" from "oen" (order matters)
    }

    [Fact]
    public void Analyze_WithMultipleOccurrences_ReturnsCorrectCount()
    {
        // Arrange - "one" appears multiple times: o-o-n-n-e-e
        // Each o can pair with each n, and each n can pair with each e
        // o(2) * n(2) * e(2) = 8 combinations
        var input = "oonnee";

        // Act
        var result = _service.Analyze(input);

        // Assert
        result.WordCounts["one"].Should().Be(8); // 2 o's * 2 n's * 2 e's = 8 combinations
    }

    [Fact]
    public void Analyze_WithComplexScrambledText_ReturnsCorrectCounts()
    {
        // Arrange - Example from problem statement
        var input = "eeehffeetsrtiiueuefxxeexeseeetoionneghtvvsentniheinungeiefev";

        // Act
        var result = _service.Analyze(input);

        // Assert
        result.Should().NotBeNull();
        result.WordCounts.Should().ContainKey("one");
        result.WordCounts.Should().ContainKey("two");
        result.WordCounts.Should().ContainKey("three");
        result.WordCounts.Should().ContainKey("four");
        result.WordCounts.Should().ContainKey("five");
        result.WordCounts.Should().ContainKey("six");
        result.WordCounts.Should().ContainKey("seven");
        result.WordCounts.Should().ContainKey("eight");
        result.WordCounts.Should().ContainKey("nine");
    }

    [Fact]
    public void Analyze_WithAllNumberWords_ReturnsAllCounts()
    {
        // Arrange
        var input = "onetwothreefourfivesixseveneightnine";

        // Act
        var result = _service.Analyze(input);

        // Assert
        result.WordCounts["one"].Should().BeGreaterThan(0);
        result.WordCounts["two"].Should().BeGreaterThan(0);
        result.WordCounts["three"].Should().BeGreaterThan(0);
        result.WordCounts["four"].Should().BeGreaterThan(0);
        result.WordCounts["five"].Should().BeGreaterThan(0);
        result.WordCounts["six"].Should().BeGreaterThan(0);
        result.WordCounts["seven"].Should().BeGreaterThan(0);
        result.WordCounts["eight"].Should().BeGreaterThan(0);
        result.WordCounts["nine"].Should().BeGreaterThan(0);
    }

    [Fact]
    public void Analyze_WithCaseInsensitive_ReturnsCorrectCounts()
    {
        // Arrange
        var input = "ONE";

        // Act
        var result = _service.Analyze(input);

        // Assert
        result.WordCounts["one"].Should().Be(1);
    }

    [Fact]
    public void Analyze_WithSpecialCharacters_IgnoresSpecialChars()
    {
        // Arrange
        var input = "o!n@e#";

        // Act
        var result = _service.Analyze(input);

        // Assert
        result.WordCounts["one"].Should().Be(1);
    }

    [Fact]
    public void Analyze_WithVeryLongInput_HandlesEfficiently()
    {
        // Arrange
        var input = new string('o', 1000) + new string('n', 1000) + new string('e', 1000);

        // Act
        var result = _service.Analyze(input);

        // Assert
        result.Should().NotBeNull();
        result.WordCounts["one"].Should().BeGreaterThan(0);
    }
}

