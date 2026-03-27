using Microsoft.AspNetCore.Mvc;
using ATS.Core.Interfaces;
using ATS.Core.Models;

namespace ATS.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AnalyzerController : ControllerBase
    {
        private readonly IResumeRepository _resumeRepository;
        private readonly IResumeAnalyzer _resumeAnalyzer;

        public AnalyzerController(IResumeRepository resumeRepository, IResumeAnalyzer resumeAnalyzer)
        {
            _resumeRepository = resumeRepository;
            _resumeAnalyzer = resumeAnalyzer;
        }

        [HttpGet("{resumeId}")]
        public async Task<IActionResult> AnalyzeResume(int resumeId)
        {
            var resume = await _resumeRepository.GetByIdAsync(resumeId);
            if (resume == null)
                return NotFound(new { message = "Resume not found" });

            var analysis = await _resumeAnalyzer.AnalyzeResumeAsync(resume);
            return Ok(analysis);
        }
    }
}