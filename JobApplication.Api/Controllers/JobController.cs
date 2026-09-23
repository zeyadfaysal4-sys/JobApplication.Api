using JobApplication.Application.DTO;
using JobApplication.Application.Featuers.Jobs.Commands.CloseJob;
using JobApplication.Application.Featuers.Jobs.Commands.CreateJob;
using JobApplication.Application.Featuers.Jobs.Commands.OpenJob;
using JobApplication.Application.Featuers.Jobs.Queries.GetAllJob;
using JobApplication.Application.Featuers.Jobs.Queries.GetById;
using JobApplication.Application.Services;
using MediatR;
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

        private readonly IMediator _mediator;

        public JobController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreateJobDto createJobDto)
        {
            var recruiterId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (recruiterId == null)
            {
                return Unauthorized();
            }

            //var jobId = await _jobServices.CreateJob(createJobDto,recruiterId);
            var jobId = await _mediator.Send(new CreateJobCommand()
            {
                Title = createJobDto.Title,
                Description = createJobDto.Description,
                IsActive = createJobDto.IsActive,
                RecruiterId = recruiterId
            });


            return Ok(new
            {
                Id = jobId
            });
        }

        /// <summary>
        /// Closes an active job.
        /// </summary>
        /// <param name="id">The ID of the job to close.</param>
        /// <returns>Confirmation that the job was closed successfully.</returns>
        /// <response code="200">Job closed successfully.</response>
        /// <response code="401">User is not authenticated.</response>
        /// <response code="403">User is not authorized to close this job.</response>
        /// <response code="404">Job was not found.</response>

        [HttpPut("{id}/close")]
        public async Task<IActionResult> CloseJob(int id)
        {
            var recruiterId =User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (recruiterId == null)
            {
                return Unauthorized();
            }

            var jobId = await _mediator.Send(new CloseJobCommand()
            {
                JobId = id,
                RecruiterId = recruiterId
            });

            return Ok(new 
            {
                message = "Job closed successfully."
            });
        }

        /// <summary>
        /// Opens an inactive job.
        /// </summary>
        /// <param name="id">The ID of the job to open.</param>
        /// <returns>Confirmation that the job was opened successfully.</returns>
        /// <response code="200">Job opened successfully.</response>
        /// <response code="401">User is not authenticated.</response>
        /// <response code="403">User is not authorized to open this job.</response>
        /// <response code="404">Job was not found.</response>

        [HttpPut("{id}/OpenJob")]
        public async Task<IActionResult> OpenJob(int id)
        {
            var recruiterId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (recruiterId == null)
            {
                return Unauthorized();
            }

             await _mediator.Send(new OpenJobCommand()
            {
                JobId = id,
                RecruiterId = recruiterId
            });

            return Ok(new
            {
                message = "Job opened successfully."
            });
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> GetAll()
        {
            //var jobs = await _jobServices.GetAllJob();
            var jobs = await _mediator.Send(new GetAllJobQuery());
            return Ok(jobs);
        }

        [HttpGet("{id}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetById(int id)
        {
            //var job = await _jobServices.GetByIdJob(id);
            var job = await _mediator.Send(new GetByIdQuery()
            {
                Id = id
            });

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