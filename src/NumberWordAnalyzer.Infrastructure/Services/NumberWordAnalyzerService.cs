using NumberWordAnalyzer.Domain.Constants;
using NumberWordAnalyzer.Domain.Models;
using NumberWordAnalyzer.Domain.Services;

namespace NumberWordAnalyzer.Infrastructure.Services;

public class NumberWordAnalyzerService : INumberWordAnalyzer
{
    public AnalysisResult Analyze(string inputText)
    {
        if (string.IsNullOrEmpty(inputText))
        {
            return new AnalysisResult();
        }

        var result = new AnalysisResult();
        var input = inputText.ToLowerInvariant();

        foreach (var word in NumberWords.All)
        {
            result.WordCounts[word] = CountSubsequenceOccurrences(input, word);
        }

        return result;
    }

    /// <summary>
    /// Counts how many times the letters of a word appear in order within the input string.
    /// This uses dynamic programming to efficiently count subsequence occurrences.
    /// </summary>
    private int CountSubsequenceOccurrences(string input, string word)
    {
        if (string.IsNullOrEmpty(word))
            return 0;

        // Dynamic programming approach: dp[i][j] = number of ways to form word[0..j-1] using input[0..i-1]
        var dp = new int[input.Length + 1, word.Length + 1];

        // Base case: empty word can be formed in 1 way (by taking nothing)
        for (int i = 0; i <= input.Length; i++)
        {
            dp[i, 0] = 1;
        }

        // Fill the DP table
        for (int i = 1; i <= input.Length; i++)
        {
            for (int j = 1; j <= word.Length; j++)
            {
                // If current input character matches current word character
                if (input[i - 1] == word[j - 1])
                {
                    // We can either:
                    // 1. Use this character (dp[i-1, j-1])
                    // 2. Skip this character (dp[i-1, j])
                    dp[i, j] = dp[i - 1, j - 1] + dp[i - 1, j];
                }
                else
                {
                    // Can't use this character, only skip it
                    dp[i, j] = dp[i - 1, j];
                }
            }
        }

        return dp[input.Length, word.Length];
    }
}

