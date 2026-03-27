using Microsoft.EntityFrameworkCore;
using ATS.Core.Entities;
using ATS.Core.Interfaces;
using ATS.Infrastructure.Data;

namespace ATS.Infrastructure.Repositories
{
    public class ResumeRepository : IResumeRepository
    {
        private readonly ATSDbContext _context;

        public ResumeRepository(ATSDbContext context)
        {
            _context = context;
        }

        public async Task<List<Resume>> GetAllAsync()
        {
            return await _context.Resumes.ToListAsync();
        }

        public async Task<Resume?> GetByIdAsync(int id)
        {
            return await _context.Resumes.FindAsync(id);
        }

        public async Task AddAsync(Resume resume)
        {
            await _context.Resumes.AddAsync(resume);
        }

        public Task UpdateAsync(Resume resume)
        {
            _context.Resumes.Update(resume);
            return Task.CompletedTask;
        }

        public async Task DeleteAsync(int id)
        {
            var resume = await GetByIdAsync(id);
            if (resume != null)
            {
                _context.Resumes.Remove(resume);
            }
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}