using Microsoft.AspNetCore.Mvc;
using ATS.Core.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace ATS.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class StatisticsController : ControllerBase
    {
        private readonly IResumeRepository _resumeRepository;
        private readonly IJobDescriptionRepository _jobRepository;
        private readonly IResumeRepository _matchRepository; // Add matches

        public StatisticsController(
            IResumeRepository resumeRepository,
            IJobDescriptionRepository jobRepository)
        {
            _resumeRepository = resumeRepository;
            _jobRepository = jobRepository;
        }

        [HttpGet]
        public async Task<IActionResult> GetStatistics()
        {
            var totalResumes = (await _resumeRepository.GetAllAsync()).Count;
            var totalJobs = (await _jobRepository.GetAllAsync()).Count;

            return Ok(new
            {
                totalResumes,
                totalJobs,
                totalMatches = 0, // Add match tracking
                averageScore = 0
            });
        }
    }
}