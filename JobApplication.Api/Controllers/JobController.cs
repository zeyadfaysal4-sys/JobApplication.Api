using JobApplication.Application.DTO;
using JobApplication.Application.Featuers.Jobs.Commands.CreateJob;
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
        private readonly JobServices _jobServices;

        private readonly IMediator _mediator;

        public JobController(JobServices jobServices, IMediator mediator)
        {
            _jobServices = jobServices;
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

        [HttpPut("{id}/close")]
        public async Task<IActionResult> Close(int id)
        {
            var recruiterId =User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (recruiterId == null)
            {
                return Unauthorized();
            }

            await _jobServices.CloseJob(id, recruiterId);

            return Ok(new 
            {
                message = "Job closed successfully."
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