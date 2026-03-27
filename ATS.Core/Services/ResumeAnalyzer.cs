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
    public class ResumeAnalyzer : IResumeAnalyzer
    {
        public async Task<ResumeAnalysis> AnalyzeResumeAsync(Resume resume)
        {
            return await Task.Run(() =>
            {
                var text = resume.ExtractedText ?? "";
                var analysis = new ResumeAnalysis();

                // 1. Format Analysis
                analysis.FormatScore = AnalyzeFormat(text);

                // 2. Length Analysis
                analysis.LengthScore = AnalyzeLength(text);

                // 3. Skills Analysis
                analysis.SkillsScore = AnalyzeSkills(text);

                // 4. Experience Analysis
                analysis.ExperienceScore = AnalyzeExperience(text);

                // 5. Education Analysis
                analysis.EducationScore = AnalyzeEducation(text);

                // 6. Overall Score
                analysis.OverallScore = (analysis.FormatScore + analysis.LengthScore +
                                         analysis.SkillsScore + analysis.ExperienceScore +
                                         analysis.EducationScore) / 5;

                // 7. Generate Improvements
                analysis.Improvements = GenerateImprovements(analysis, text);

                // 8. Detect Sections
                analysis.Sections = DetectSections(text);

                return analysis;
            });
        }

        private int AnalyzeFormat(string text)
        {
            int score = 0;

            // Check for bullet points
            if (Regex.IsMatch(text, @"[•\-\*]\s+\w"))
                score += 25;

            // Check for sections (experience, education, skills)
            if (Regex.IsMatch(text, @"(?i)(experience|work|employment)"))
                score += 25;
            if (Regex.IsMatch(text, @"(?i)(education|degree|university|college)"))
                score += 25;
            if (Regex.IsMatch(text, @"(?i)(skills|technologies|competencies)"))
                score += 25;

            return score;
        }

        private int AnalyzeLength(string text)
        {
            var wordCount = text.Split(new[] { ' ', '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries).Length;

            if (wordCount >= 400 && wordCount <= 800) // 1-2 pages
                return 100;
            else if (wordCount >= 200 && wordCount < 400)
                return 75;
            else if (wordCount >= 800 && wordCount <= 1200)
                return 50;
            else
                return 25;
        }

        private int AnalyzeSkills(string text)
        {
            var techSkills = new[] { "c#", "java", "python", "javascript", "sql", "azure", "aws",
                                      "docker", "kubernetes", "react", "angular", "node", "net",
                                      "asp", "entity", "framework", "rest", "api", "microservices" };

            var foundSkills = techSkills.Count(skill => text.ToLower().Contains(skill));

            if (foundSkills >= 10)
                return 100;
            else if (foundSkills >= 7)
                return 75;
            else if (foundSkills >= 4)
                return 50;
            else
                return 25;
        }

        private int AnalyzeExperience(string text)
        {
            // Look for experience indicators
            var years = Regex.Matches(text, @"\d+\+?\s*(?:years?|yrs?)", RegexOptions.IgnoreCase);
            var experienceWords = new[] { "senior", "lead", "architect", "manager", "director" };
            var hasExperience = experienceWords.Any(word => text.ToLower().Contains(word));

            if (years.Count >= 3 || hasExperience)
                return 100;
            else if (years.Count == 2)
                return 75;
            else if (years.Count == 1)
                return 50;
            else
                return 25;
        }

        private int AnalyzeEducation(string text)
        {
            var educationKeywords = new[] { "bachelor", "master", "degree", "university", "college",
                                            "b.tech", "b.e", "m.tech", "phd", "diploma" };

            var found = educationKeywords.Count(keyword => text.ToLower().Contains(keyword));

            if (found >= 3)
                return 100;
            else if (found == 2)
                return 75;
            else if (found == 1)
                return 50;
            else
                return 25;
        }

        private List<string> GenerateImprovements(ResumeAnalysis analysis, string text)
        {
            var improvements = new List<string>();

            if (analysis.FormatScore < 60)
            {
                improvements.Add("📝 Use bullet points to highlight achievements");
                improvements.Add("📑 Add clear section headers (Experience, Skills, Education)");
            }

            if (analysis.LengthScore < 60)
            {
                if (text.Split().Length < 200)
                    improvements.Add("📄 Your resume is too short. Aim for 1-2 pages with detailed experience");
                else
                    improvements.Add("📄 Your resume is too long. Keep it to 1-2 pages max");
            }

            if (analysis.SkillsScore < 60)
            {
                improvements.Add("💡 Add a dedicated 'Technical Skills' section");
                improvements.Add("🔧 Include more relevant technologies and tools");
            }

            if (analysis.ExperienceScore < 60)
            {
                improvements.Add("💼 Add years of experience in summary");
                improvements.Add("📊 Quantify achievements with numbers (e.g., 'Improved performance by 30%')");
            }

            if (analysis.EducationScore < 60)
            {
                improvements.Add("🎓 Add education details (degree, institution, year)");
            }

            return improvements;
        }

        private Dictionary<string, List<string>> DetectSections(string text)
        {
            var sections = new Dictionary<string, List<string>>();
            var lines = text.Split('\n');

            string currentSection = "Other";
            sections[currentSection] = new List<string>();

            foreach (var line in lines)
            {
                var lowerLine = line.ToLower().Trim();

                if (lowerLine.Contains("experience") || lowerLine.Contains("work"))
                    currentSection = "Experience";
                else if (lowerLine.Contains("education") || lowerLine.Contains("degree"))
                    currentSection = "Education";
                else if (lowerLine.Contains("skill") || lowerLine.Contains("technology"))
                    currentSection = "Skills";
                else if (lowerLine.Contains("project"))
                    currentSection = "Projects";
                else if (lowerLine.Contains("certification"))
                    currentSection = "Certifications";

                if (!sections.ContainsKey(currentSection))
                    sections[currentSection] = new List<string>();

                if (!string.IsNullOrWhiteSpace(line) && line.Length > 10)
                    sections[currentSection].Add(line.Trim());
            }

            return sections;
        }
    }
}