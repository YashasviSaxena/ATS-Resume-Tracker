using Microsoft.AspNetCore.Mvc;
using ATS.Core.Interfaces;
using ATS.Core.Entities;
using ATS.API.DTOs;

namespace ATS.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class JobDescriptionController : ControllerBase
    {
        private readonly IJobDescriptionRepository _jobRepository;

        public JobDescriptionController(IJobDescriptionRepository jobRepository)
        {
            _jobRepository = jobRepository;
        }

        // GET: api/jobdescription
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var jobs = await _jobRepository.GetAllAsync();
            var jobDTOs = jobs.Select(j => new JobDescriptionDTO
            {
                Id = j.Id,
                Title = j.Title,
                Description = j.Description?.Length > 200 ? j.Description.Substring(0, 200) + "..." : j.Description,
                Company = j.Company,
                Location = j.Location,
                CreatedAt = j.CreatedAt
            });

            return Ok(jobDTOs);
        }

        // GET: api/jobdescription/{id}
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var job = await _jobRepository.GetByIdAsync(id);

            if (job == null)
                return NotFound(new { message = "Job not found" });

            var jobDTO = new JobDescriptionDTO
            {
                Id = job.Id,
                Title = job.Title,
                Description = job.Description,
                Company = job.Company,
                Location = job.Location,
                CreatedAt = job.CreatedAt
            };

            return Ok(jobDTO);
        }

        // POST: api/jobdescription
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateJobDescriptionDTO jobDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var job = new JobDescription
            {
                Title = jobDto.Title,
                Description = jobDto.Description,
                Company = jobDto.Company,
                Location = jobDto.Location,
                CreatedAt = DateTime.UtcNow
            };

            await _jobRepository.AddAsync(job);
            await _jobRepository.SaveChangesAsync();

            return CreatedAtAction(nameof(GetById), new { id = job.Id }, job);
        }

        // PUT: api/jobdescription/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateJobDescriptionDTO jobDto)
        {
            var job = await _jobRepository.GetByIdAsync(id);

            if (job == null)
                return NotFound(new { message = "Job not found" });

            job.Title = jobDto.Title;
            job.Description = jobDto.Description;
            job.Company = jobDto.Company;
            job.Location = jobDto.Location;

            await _jobRepository.UpdateAsync(job);
            await _jobRepository.SaveChangesAsync();

            return Ok(job);
        }

        // DELETE: api/jobdescription/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var job = await _jobRepository.GetByIdAsync(id);

            if (job == null)
                return NotFound(new { message = "Job not found" });

            await _jobRepository.DeleteAsync(id);
            await _jobRepository.SaveChangesAsync();

            return Ok(new { message = "Job deleted successfully" });
        }
    }
}