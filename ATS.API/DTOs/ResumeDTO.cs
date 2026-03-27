namespace ATS.API.DTOs
{
    public class ResumeDTO
    {
        public int Id { get; set; }
        public string FileName { get; set; } = string.Empty;
        public string FilePath { get; set; } = string.Empty;
        public string ExtractedText { get; set; } = string.Empty;
        public DateTime UploadedAt { get; set; }
    }

    public class ResumeDetailDTO
    {
        public int Id { get; set; }
        public string FileName { get; set; } = string.Empty;
        public string ExtractedText { get; set; } = string.Empty;
        public DateTime UploadedAt { get; set; }
    }
}