using ATS.Core.Entities;

namespace ATS.Core.Interfaces
{
    public interface IJobDescriptionRepository
    {
        Task<List<JobDescription>> GetAllAsync();
        Task<JobDescription?> GetByIdAsync(int id);
        Task AddAsync(JobDescription job);
        Task UpdateAsync(JobDescription job);
        Task DeleteAsync(int id);
        Task SaveChangesAsync();
    }
}