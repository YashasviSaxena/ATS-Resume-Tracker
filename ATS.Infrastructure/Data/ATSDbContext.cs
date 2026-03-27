using Microsoft.EntityFrameworkCore;
using ATS.Core.Entities;

namespace ATS.Infrastructure.Data
{
    public class ATSDbContext : DbContext
    {
        public ATSDbContext(DbContextOptions<ATSDbContext> options) : base(options)
        {
        }

        public DbSet<Resume> Resumes { get; set; }
        public DbSet<JobDescription> Jobs { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configure table names if needed
            modelBuilder.Entity<Resume>().ToTable("Resumes");
            modelBuilder.Entity<JobDescription>().ToTable("JobDescriptions");
        }
    }
}