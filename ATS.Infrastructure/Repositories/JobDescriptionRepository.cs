using Microsoft.EntityFrameworkCore;
using ATS.Core.Entities;
using ATS.Core.Interfaces;
using ATS.Infrastructure.Data;

namespace ATS.Infrastructure.Repositories
{
    public class JobDescriptionRepository : IJobDescriptionRepository
    {
        private readonly ATSDbContext _context;

        public JobDescriptionRepository(ATSDbContext context)
        {
            _context = context;
        }

        public async Task<List<JobDescription>> GetAllAsync()
        {
            return await _context.Jobs.ToListAsync();
        }

        public async Task<JobDescription?> GetByIdAsync(int id)
        {
            return await _context.Jobs.FindAsync(id);
        }

        public async Task AddAsync(JobDescription job)
        {
            await _context.Jobs.AddAsync(job);
        }

        public Task UpdateAsync(JobDescription job)
        {
            _context.Jobs.Update(job);
            return Task.CompletedTask;
        }

        public async Task DeleteAsync(int id)
        {
            var job = await GetByIdAsync(id);
            if (job != null)
            {
                _context.Jobs.Remove(job);
            }
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}