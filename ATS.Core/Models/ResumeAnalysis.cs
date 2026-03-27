namespace ATS.Core.Models
{
    public class ResumeAnalysis
    {
        public int FormatScore { get; set; }
        public int LengthScore { get; set; }
        public int SkillsScore { get; set; }
        public int ExperienceScore { get; set; }
        public int EducationScore { get; set; }
        public int OverallScore { get; set; }
        public List<string> Improvements { get; set; } = new();
        public Dictionary<string, List<string>> Sections { get; set; } = new();
    }
}