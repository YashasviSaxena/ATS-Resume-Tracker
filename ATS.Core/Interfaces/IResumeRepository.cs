using ATS.Core.Entities;

namespace ATS.Core.Interfaces
{
    public interface IResumeRepository
    {
        Task<List<Resume>> GetAllAsync();
        Task<Resume?> GetByIdAsync(int id);
        Task AddAsync(Resume resume);
        Task UpdateAsync(Resume resume);
        Task DeleteAsync(int id);
        Task SaveChangesAsync();
    }
}