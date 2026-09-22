using JobApplication.Application.DTO;
using JobApplication.Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace JobApplication.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Recruiter")]
    public class JobController : ControllerBase
    {
        private readonly JobServices _jobServices;

        public JobController(JobServices jobServices)
        {
            _jobServices = jobServices;
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreateJobDto createJobDto)
        {
            var recruiterId =
                User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (recruiterId == null)
            {
                return Unauthorized();
            }

            var jobId = await _jobServices.Create(createJobDto,recruiterId);

            return Ok(new
            {
                Id = jobId
            });
        }

        [HttpPut("{id}/close")]
        public async Task<IActionResult> Close(int id)
        {
            var recruiterId =User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (recruiterId == null)
            {
                return Unauthorized();
            }

            await _jobServices.Close(id, recruiterId);

            return Ok(new 
            {
                message = "Job closed successfully."
            });
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> GetAll()
        {
            var jobs = await _jobServices.GetAll();
            return Ok(jobs);
        }

        [HttpGet("{id}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetById(int id)
        {
            var job = await _jobServices.GetById(id);

            if (job == null)
            {
                return NotFound(new 
                { 
                    message = "Job not found." 
                });
            }

            return Ok(job);
        }
    }
}