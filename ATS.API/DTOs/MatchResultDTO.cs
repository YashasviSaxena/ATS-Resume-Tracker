namespace ATS.API.DTOs
{
    public class MatchRequestDTO
    {
        public int ResumeId { get; set; }
        public int JobId { get; set; }
    }

    public class MatchResponseDTO
    {
        public int? ResumeId { get; set; }
        public string? ResumeFileName { get; set; }
        public int? JobId { get; set; }
        public string? JobTitle { get; set; }
        public float Score { get; set; }
        public string MatchLevel { get; set; } = string.Empty;
        public bool IsGoodMatch { get; set; }
        public List<string> MissingKeywords { get; set; } = new();
        public List<string> Suggestions { get; set; } = new();
        public List<string> ResumeKeywords { get; set; } = new();
        public List<string> JobKeywords { get; set; } = new();
    }
}