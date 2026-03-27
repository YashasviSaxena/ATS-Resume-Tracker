using ATS.Core.Entities;
using ATS.Core.Models;

namespace ATS.Core.Interfaces
{
    public interface IResumeAnalyzer
    {
        Task<ResumeAnalysis> AnalyzeResumeAsync(Resume resume);
    }
}