using NumberWordAnalyzer.Domain.Models;

namespace NumberWordAnalyzer.Domain.Services;

public interface INumberWordAnalyzer
{
    AnalysisResult Analyze(string inputText);
}

