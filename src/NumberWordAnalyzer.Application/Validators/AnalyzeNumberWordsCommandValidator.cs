using FluentValidation;
using NumberWordAnalyzer.Application.Commands;

namespace NumberWordAnalyzer.Application.Validators;

public class AnalyzeNumberWordsCommandValidator : AbstractValidator<AnalyzeNumberWordsCommand>
{
    public AnalyzeNumberWordsCommandValidator()
    {
        RuleFor(x => x.InputText)
            .NotNull()
            .WithMessage("Input text cannot be null.")
            .NotEmpty()
            .WithMessage("Input text cannot be empty.");
    }
}

