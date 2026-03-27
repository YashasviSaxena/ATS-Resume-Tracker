using System;
using System.Text;
using ATS.Core.Interfaces;
using UglyToad.PdfPig;

namespace ATS.Core.Services
{
    public class PdfParser : IResumeParser
    {
        public string ExtractText(string filePath)
        {
            try
            {
                using (var pdf = PdfDocument.Open(filePath))
                {
                    var text = new StringBuilder();
                    foreach (var page in pdf.GetPages())
                    {
                        text.AppendLine(page.Text);
                    }
                    return text.ToString();
                }
            }
            catch (Exception ex)
            {
                return $"Error extracting text from PDF: {ex.Message}";
            }
        }
    }
}