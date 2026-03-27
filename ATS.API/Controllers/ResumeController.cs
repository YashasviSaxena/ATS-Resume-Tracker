using Microsoft.AspNetCore.Mvc;
using ATS.Core.Interfaces;
using ATS.Core.Entities;
using ATS.API.DTOs;

namespace ATS.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ResumeController : ControllerBase
    {
        private readonly IResumeRepository _resumeRepository;
        private readonly IWebHostEnvironment _environment;
        private readonly IResumeParser _resumeParser;

        public ResumeController(
            IResumeRepository resumeRepository,
            IWebHostEnvironment environment,
            IResumeParser resumeParser)
        {
            _resumeRepository = resumeRepository;
            _environment = environment;
            _resumeParser = resumeParser;
        }

        // GET: api/resume
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var resumes = await _resumeRepository.GetAllAsync();
            var resumeDTOs = resumes.Select(r => new ResumeDTO
            {
                Id = r.Id,
                FileName = r.FileName,
                FilePath = r.FilePath,
                ExtractedText = r.ExtractedText?.Length > 100 ? r.ExtractedText.Substring(0, 100) + "..." : r.ExtractedText,
                UploadedAt = r.UploadedAt
            });

            return Ok(resumeDTOs);
        }

        // GET: api/resume/{id}
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var resume = await _resumeRepository.GetByIdAsync(id);

            if (resume == null)
                return NotFound(new { message = "Resume not found" });

            var resumeDTO = new ResumeDetailDTO
            {
                Id = resume.Id,
                FileName = resume.FileName,
                ExtractedText = resume.ExtractedText,
                UploadedAt = resume.UploadedAt
            };

            return Ok(resumeDTO);
        }

        // POST: api/resume/upload
        [HttpPost("upload")]
        public async Task<IActionResult> Upload([FromForm] IFormFile file)
        {
            if (file == null || file.Length == 0)
                return BadRequest(new { message = "No file uploaded" });

            var allowedExtensions = new[] { ".pdf", ".doc", ".docx", ".txt" };
            var fileExtension = Path.GetExtension(file.FileName).ToLower();

            if (!allowedExtensions.Contains(fileExtension))
                return BadRequest(new { message = "Only PDF, DOC, DOCX, and TXT files are allowed" });

            try
            {
                var uploadsPath = Path.Combine(_environment.ContentRootPath, "uploads");
                if (!Directory.Exists(uploadsPath))
                    Directory.CreateDirectory(uploadsPath);

                var fileName = $"{Guid.NewGuid()}_{file.FileName}";
                var filePath = Path.Combine(uploadsPath, fileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                }

                // Extract text using the parser
                string extractedText;
                if (fileExtension == ".txt")
                {
                    // For text files, read directly
                    extractedText = System.IO.File.ReadAllText(filePath);
                }
                else
                {
                    // For PDF/DOCX, use the parser
                    extractedText = _resumeParser.ExtractText(filePath);
                }

                var resume = new Resume
                {
                    FileName = file.FileName,
                    FilePath = filePath,
                    ExtractedText = extractedText,
                    UploadedAt = DateTime.UtcNow
                };

                await _resumeRepository.AddAsync(resume);
                await _resumeRepository.SaveChangesAsync();

                return Ok(new
                {
                    id = resume.Id,
                    fileName = resume.FileName,
                    extractedText = extractedText.Length > 200 ? extractedText.Substring(0, 200) + "..." : extractedText,
                    uploadedAt = resume.UploadedAt,
                    message = "Resume uploaded successfully"
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = $"Error uploading file: {ex.Message}" });
            }
        }

        // DELETE: api/resume/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var resume = await _resumeRepository.GetByIdAsync(id);

            if (resume == null)
                return NotFound(new { message = "Resume not found" });

            if (System.IO.File.Exists(resume.FilePath))
                System.IO.File.Delete(resume.FilePath);

            await _resumeRepository.DeleteAsync(id);
            await _resumeRepository.SaveChangesAsync();

            return Ok(new { message = "Resume deleted successfully" });
        }
    }
}