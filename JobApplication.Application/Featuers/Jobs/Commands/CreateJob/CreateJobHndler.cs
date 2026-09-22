using JobApplication.Application.DTO;
using JobApplication.Application.Interfaces;
using JobApplication.Domin.Entities;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace JobApplication.Application.Featuers.Jobs.Commands.CreateJob
{
    public class CreateJobHndler : IRequestHandler<CreateJobCommand, int>
    {
        private readonly IRepository<Job> _jobRepository;

        public CreateJobHndler(IRepository<Job> jobRepository)
        {
            _jobRepository = jobRepository;
        }

        public async Task<int> Handle(CreateJobCommand request, CancellationToken cancellationToken)
        {
            var job = new Job()
            {
                Title = request.Title,
                Description = request.Description,
                IsActive = request.IsActive,
                RecruiterId = request.RecruiterId
            };

            await _jobRepository.AddAsync(job);
            await _jobRepository.CommitAsync();

            return job.Id;
        }
    }
}
