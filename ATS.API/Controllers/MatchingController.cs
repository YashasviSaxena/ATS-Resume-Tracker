using Microsoft.AspNetCore.Mvc;
using ATS.Core.Interfaces;
using ATS.API.DTOs;

namespace ATS.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class MatchingController : ControllerBase
    {
        private readonly IResumeRepository _resumeRepository;
        private readonly IJobDescriptionRepository _jobRepository;
        private readonly IMatchingService _matchingService;

        public MatchingController(
            IResumeRepository resumeRepository,
            IJobDescriptionRepository jobRepository,
            IMatchingService matchingService)
        {
            _resumeRepository = resumeRepository;
            _jobRepository = jobRepository;
            _matchingService = matchingService;
        }

        [HttpPost("compare")]
        public async Task<IActionResult> Compare([FromBody] MatchRequestDTO matchRequest)
        {
            var resume = await _resumeRepository.GetByIdAsync(matchRequest.ResumeId);
            var job = await _jobRepository.GetByIdAsync(matchRequest.JobId);

            if (resume == null) return NotFound($"Resume with ID {matchRequest.ResumeId} not found");
            if (job == null) return NotFound($"Job with ID {matchRequest.JobId} not found");

            var result = await _matchingService.CalculateMatchAsync(resume, job);

            return Ok(new MatchResponseDTO
            {
                Score = result.Score,
                MatchLevel = result.MatchLevel,
                IsGoodMatch = result.IsGoodMatch,
                MissingKeywords = result.MissingKeywords,
                Suggestions = result.Suggestions,
                ResumeKeywords = result.ResumeKeywords,
                JobKeywords = result.JobKeywords
            });
        }

        [HttpGet("resume/{resumeId}/jobs")]
        public async Task<IActionResult> GetMatchesForResume(int resumeId)
        {
            var resume = await _resumeRepository.GetByIdAsync(resumeId);
            if (resume == null) return NotFound($"Resume with ID {resumeId} not found");

            var jobs = await _jobRepository.GetAllAsync();
            var results = new List<MatchResponseDTO>();

            foreach (var job in jobs)
            {
                var result = await _matchingService.CalculateMatchAsync(resume, job);
                results.Add(new MatchResponseDTO
                {
                    JobId = job.Id,
                    JobTitle = job.Title,
                    Score = result.Score,
                    MatchLevel = result.MatchLevel,
                    IsGoodMatch = result.IsGoodMatch,
                    MissingKeywords = result.MissingKeywords.Take(5).ToList()
                });
            }

            return Ok(results.OrderByDescending(r => r.Score));
        }
    }
}