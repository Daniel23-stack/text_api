using MediatR;
using NumberWordAnalyzer.Application.Commands;
using NumberWordAnalyzer.Domain.Models;
using NumberWordAnalyzer.Domain.Services;

namespace NumberWordAnalyzer.Application.Handlers;

public class AnalyzeNumberWordsCommandHandler : IRequestHandler<AnalyzeNumberWordsCommand, AnalysisResult>
{
    private readonly INumberWordAnalyzer _analyzer;

    public AnalyzeNumberWordsCommandHandler(INumberWordAnalyzer analyzer)
    {
        _analyzer = analyzer;
    }

    public Task<AnalysisResult> Handle(AnalyzeNumberWordsCommand request, CancellationToken cancellationToken)
    {
        var result = _analyzer.Analyze(request.InputText);
        return Task.FromResult(result);
    }
}

