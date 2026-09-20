using JobApplication.Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace JobApplication.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Candidate")]
    public class JobApplicationController : ControllerBase
    {
        private readonly JobApplicationServices _jobApplicationServices;

        public JobApplicationController(
            JobApplicationServices jobApplicationServices)
        {
            _jobApplicationServices = jobApplicationServices;
        }

        [HttpPost("apply")]
        public async Task<IActionResult> Apply(int jobId)
        {
            var applicationUserId =
                User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (applicationUserId == null)
            {
                return Unauthorized();
            }

            await _jobApplicationServices.Apply(jobId,applicationUserId);

            return Ok("Application submitted successfully.");
        }

        [HttpDelete("{applicationId}")]
        public async Task<IActionResult> Cancel(int applicationId)
        {
            var applicationUserId =User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (applicationUserId == null)
            {
                return Unauthorized();
            }

            await _jobApplicationServices.Cancel(applicationId,applicationUserId);

            return Ok("Application cancelled successfully.");
        }
    }
}