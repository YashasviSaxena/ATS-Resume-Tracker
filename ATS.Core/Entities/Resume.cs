using System;

namespace ATS.Core.Entities
{
    public class Resume
    {
        public int Id { get; set; }
        public string FileName { get; set; } = string.Empty;
        public string FilePath { get; set; } = string.Empty;
        public string ExtractedText { get; set; } = string.Empty; 
        public DateTime UploadedAt { get; set; } = DateTime.UtcNow;
    }
}