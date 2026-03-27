using ATS.Core.Entities;
using ATS.Core.Models;

namespace ATS.Core.Interfaces
{
    public interface IMatchingService
    {
        Task<MatchResult> CalculateMatchAsync(Resume resume, JobDescription job);
        Task<List<string>> ExtractKeywordsAsync(string text);
        Task<float> CalculateScoreAsync(List<string> resumeKeywords, List<string> jobKeywords);
        Task<List<string>> FindMissingKeywordsAsync(List<string> resumeKeywords, List<string> jobKeywords);
        Task<List<string>> GenerateSuggestionsAsync(List<string> missingKeywords);
    }
}