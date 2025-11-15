using NumberWordAnalyzer.Domain.Constants;

namespace NumberWordAnalyzer.Domain.Models;

public class AnalysisResult
{
    public Dictionary<string, int> WordCounts { get; set; } = new();

    public AnalysisResult()
    {
        // Initialize with all number words set to 0
        foreach (var word in NumberWords.All)
        {
            WordCounts[word] = 0;
        }
    }
}

