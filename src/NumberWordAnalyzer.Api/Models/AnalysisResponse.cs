namespace NumberWordAnalyzer.Api.Models;

public class AnalysisResponse
{
    public Dictionary<string, int> WordCounts { get; set; } = new();
}

