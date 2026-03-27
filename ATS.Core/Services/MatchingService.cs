using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using ATS.Core.Entities;
using ATS.Core.Interfaces;
using ATS.Core.Models;

namespace ATS.Core.Services
{
    public class MatchingService : IMatchingService
    {
        private readonly HashSet<string> _stopWords = new()
        {
            "a", "an", "and", "are", "as", "at", "be", "but", "by", "for", "in", "into", "is", "it",
            "no", "not", "of", "on", "or", "such", "that", "the", "their", "then", "there", "these",
            "they", "this", "to", "was", "will", "with", "i", "you", "we", "our", "your", "have", "has",
            "had", "do", "does", "did", "can", "could", "would", "should", "may", "might", "like", "just",
            "more", "most", "some", "any", "etc", "eg", "ie", "also", "well", "get", "make", "take",
            "using", "use", "used", "via", "using", "based", "using"
        };

        public async Task<MatchResult> CalculateMatchAsync(Resume resume, JobDescription job)
        {
            return await Task.Run(() =>
            {
                var resumeKeywords = ExtractKeywords(resume.ExtractedText ?? "");
                var jobKeywords = ExtractKeywords(job.Description ?? "");

                // Use TF-IDF for better scoring
                var score = CalculateTFIDFScore(resumeKeywords, jobKeywords);
                var missingKeywords = FindMissingKeywords(resumeKeywords, jobKeywords);
                var suggestions = GenerateSuggestions(missingKeywords, score);

                return new MatchResult
                {
                    Score = score,
                    ResumeKeywords = resumeKeywords.Take(30).ToList(),
                    JobKeywords = jobKeywords.Take(30).ToList(),
                    MissingKeywords = missingKeywords.Take(15).ToList(),
                    Suggestions = suggestions
                };
            });
        }

        public async Task<List<string>> ExtractKeywordsAsync(string text)
        {
            return await Task.Run(() => ExtractKeywords(text ?? ""));
        }

        public async Task<float> CalculateScoreAsync(List<string> resumeKeywords, List<string> jobKeywords)
        {
            return await Task.Run(() => CalculateBasicScore(resumeKeywords, jobKeywords));
        }

        public async Task<float> CalculateTFIDFScoreAsync(List<string> resumeKeywords, List<string> jobKeywords)
        {
            return await Task.Run(() => CalculateTFIDFScore(resumeKeywords, jobKeywords));
        }

        public async Task<List<string>> FindMissingKeywordsAsync(List<string> resumeKeywords, List<string> jobKeywords)
        {
            return await Task.Run(() => FindMissingKeywords(resumeKeywords, jobKeywords));
        }

        public async Task<List<string>> GenerateSuggestionsAsync(List<string> missingKeywords)
        {
            return await Task.Run(() => GenerateSuggestions(missingKeywords, 0));
        }

        // Synchronous helper methods
        private List<string> ExtractKeywords(string text)
        {
            if (string.IsNullOrWhiteSpace(text))
                return new List<string>();

            var lowerText = text.ToLower();

            // Remove special characters and split into words
            var words = Regex.Split(lowerText, @"[^a-zA-Z0-9\s-]")
                .SelectMany(w => w.Split(new[] { ' ', '\t', '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries))
                .Where(w => w.Length > 2)
                .Where(w => !_stopWords.Contains(w))
                .Where(w => !IsNumeric(w))
                .Where(w => !IsCommonWord(w))
                .ToList();

            // Extract bigrams (two-word phrases) for better context
            var bigrams = new List<string>();
            for (int i = 0; i < words.Count - 1; i++)
            {
                var bigram = $"{words[i]}_{words[i + 1]}";
                if (IsMeaningfulBigram(words[i], words[i + 1]))
                {
                    bigrams.Add(bigram);
                }
            }

            // Combine unigrams and bigrams
            var allTerms = new List<string>();
            allTerms.AddRange(words);
            allTerms.AddRange(bigrams);

            // Count frequency
            var termFrequency = allTerms
                .GroupBy(t => t)
                .ToDictionary(g => g.Key, g => g.Count());

            // Sort by frequency and take top keywords
            var keywords = termFrequency
                .OrderByDescending(kv => kv.Value)
                .Select(kv => kv.Key)
                .ToList();

            return keywords;
        }

        private float CalculateBasicScore(List<string> resumeKeywords, List<string> jobKeywords)
        {
            if (jobKeywords.Count == 0) return 0f;
            var matchCount = resumeKeywords.Intersect(jobKeywords, StringComparer.OrdinalIgnoreCase).Count();
            return (float)Math.Round((float)matchCount / jobKeywords.Count * 100, 2);
        }

        private float CalculateTFIDFScore(List<string> resumeKeywords, List<string> jobKeywords)
        {
            if (jobKeywords.Count == 0) return 0f;

            // Calculate Term Frequency for resume
            var tf = resumeKeywords
                .GroupBy(k => k)
                .ToDictionary(g => g.Key, g => (double)g.Count() / resumeKeywords.Count);

            // Calculate Inverse Document Frequency (using job description as corpus)
            var idf = jobKeywords
                .GroupBy(k => k)
                .ToDictionary(g => g.Key, g => Math.Log((double)jobKeywords.Count / g.Count()));

            // Calculate TF-IDF weighted score
            double totalScore = 0;
            foreach (var jobKeyword in jobKeywords)
            {
                var tfidf = tf.GetValueOrDefault(jobKeyword, 0) * idf.GetValueOrDefault(jobKeyword, 1);
                totalScore += tfidf;
            }

            // Normalize to percentage
            var percentage = (totalScore / jobKeywords.Count) * 100;
            return (float)Math.Round(percentage, 2);
        }

        private List<string> FindMissingKeywords(List<string> resumeKeywords, List<string> jobKeywords)
        {
            return jobKeywords
                .Except(resumeKeywords, StringComparer.OrdinalIgnoreCase)
                .Take(20)
                .ToList();
        }

        private List<string> GenerateSuggestions(List<string> missingKeywords, float score)
        {
            var suggestions = new List<string>();

            if (score >= 80)
            {
                suggestions.Add("🎉 Excellent match! Your resume aligns very well with this position.");
                suggestions.Add("✨ Consider highlighting your most impressive achievements in the interview.");
            }
            else if (score >= 60)
            {
                suggestions.Add("👍 Good match! Your skills align well with most requirements.");
                suggestions.Add("💡 To improve your match, consider adding these keywords:");
                foreach (var keyword in missingKeywords.Take(5))
                {
                    suggestions.Add($"   • Add '{keyword.Replace("_", " ")}' to your skills section");
                }
            }
            else if (score >= 40)
            {
                suggestions.Add("📊 Fair match. Your resume shows some relevant skills.");
                suggestions.Add("💡 Recommendations to improve:");
                suggestions.Add("   • Customize your resume for this specific role");
                suggestions.Add("   • Add more relevant keywords from the job description");
                suggestions.Add($"   • Focus on: {string.Join(", ", missingKeywords.Take(5).Select(k => k.Replace("_", " ")))}");
            }
            else
            {
                suggestions.Add("⚠️ Your resume needs significant improvement for this position.");
                suggestions.Add("💡 Action Items:");
                suggestions.Add("   • Review the job description carefully");
                suggestions.Add("   • Highlight relevant experience and skills");
                suggestions.Add("   • Add missing technical keywords");
                suggestions.Add($"   • Key skills to add: {string.Join(", ", missingKeywords.Take(8).Select(k => k.Replace("_", " ")))}");
            }

            // Add general improvement tips
            if (missingKeywords.Count > 10)
            {
                suggestions.Add("📝 Consider creating a skills section that lists your core competencies.");
            }

            if (score > 0 && score < 50)
            {
                suggestions.Add("🎯 Tailor your resume for each application - customize the summary and skills section.");
            }

            return suggestions;
        }

        private bool IsNumeric(string word)
        {
            return int.TryParse(word, out _) || double.TryParse(word, out _);
        }

        private bool IsCommonWord(string word)
        {
            var commonWords = new HashSet<string>
            {
                "work", "year", "time", "team", "project", "company", "experience", "skill",
                "responsibility", "task", "role", "position", "job", "career", "professional"
            };
            return commonWords.Contains(word);
        }

        private bool IsMeaningfulBigram(string word1, string word2)
        {
            // Only create bigrams for meaningful combinations
            var techWords = new HashSet<string> { "c", "c#", "java", "python", "javascript", "react", "angular", "node", "net", "asp", "sql", "azure", "aws", "docker", "kubernetes" };
            return techWords.Contains(word1) || techWords.Contains(word2);
        }
    }
}