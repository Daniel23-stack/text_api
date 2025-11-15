using MediatR;
using NumberWordAnalyzer.Domain.Models;

namespace NumberWordAnalyzer.Application.Commands;

public class AnalyzeNumberWordsCommand : IRequest<AnalysisResult>
{
    public string InputText { get; set; } = string.Empty;
}

