
using JobApplication.Application.Featuers.JobApplications.Commands.ApplyJobApplication;
using JobApplication.Application.Featuers.JobApplications.Commands.CancelJobApplication;
using JobApplication.Application.Services;
using MediatR;
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
        private readonly IMediator _mediator;

        public JobApplicationController(
            JobApplicationServices jobApplicationServices,
            IMediator mediator)
        {
            _jobApplicationServices = jobApplicationServices;
            _mediator = mediator;
        }

        [HttpPost("apply")]
        public async Task<IActionResult> Apply(int jobId)
        {
            var applicationUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (applicationUserId == null)
            {
                return Unauthorized();
            }

            // var result = await _jobApplicationServices.Apply(jobId, applicationUserId);
            await _mediator.Send(new ApplyJobApplicationCommand()
            {
                JobId = jobId,
                ApplicationUserId = applicationUserId
            });

            return Ok("Application submitted successfully.");
        }

        [HttpDelete("{applicationId}")]
        public async Task<IActionResult> Cancel(int applicationId)
        {
            var applicationUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (applicationUserId == null)
            {
                return Unauthorized();
            }

            // await _jobApplicationServices.Cancel(applicationId, applicationUserId);
            await _mediator.Send(new CancelJobApplicationCommand
            {
                ApplicationId = applicationId,
                ApplicationUserId = applicationUserId
            });

            return Ok("Application cancelled successfully.");
        }
    }
}