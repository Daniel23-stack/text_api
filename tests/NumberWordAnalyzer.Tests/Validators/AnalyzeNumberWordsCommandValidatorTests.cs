using FluentAssertions;
using FluentValidation.TestHelper;
using NumberWordAnalyzer.Application.Commands;
using NumberWordAnalyzer.Application.Validators;
using Xunit;

namespace NumberWordAnalyzer.Tests.Validators;

public class AnalyzeNumberWordsCommandValidatorTests
{
    private readonly AnalyzeNumberWordsCommandValidator _validator;

    public AnalyzeNumberWordsCommandValidatorTests()
    {
        _validator = new AnalyzeNumberWordsCommandValidator();
    }

    [Fact]
    public void Validate_WithValidInput_ShouldPass()
    {
        // Arrange
        var command = new AnalyzeNumberWordsCommand { InputText = "test" };

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Validate_WithNullInput_ShouldFail()
    {
        // Arrange
        var command = new AnalyzeNumberWordsCommand { InputText = null! };

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.InputText);
    }

    [Fact]
    public void Validate_WithEmptyInput_ShouldFail()
    {
        // Arrange
        var command = new AnalyzeNumberWordsCommand { InputText = string.Empty };

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.InputText);
    }

    [Fact]
    public void Validate_WithWhitespaceOnly_ShouldFail()
    {
        // Arrange
        var command = new AnalyzeNumberWordsCommand { InputText = "   " };

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.InputText);
    }
}

