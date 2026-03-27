namespace ATS.Core.Models
{
    public class MatchResult
    {
        public float Score { get; set; }
        public List<string> ResumeKeywords { get; set; } = new();
        public List<string> JobKeywords { get; set; } = new();
        public List<string> MissingKeywords { get; set; } = new();
        public List<string> Suggestions { get; set; } = new();

        public bool IsGoodMatch => Score >= 60;

        public string MatchLevel => Score switch
        {
            >= 80 => "Excellent",
            >= 60 => "Good",
            >= 40 => "Fair",
            _ => "Poor"
        };
    }
}