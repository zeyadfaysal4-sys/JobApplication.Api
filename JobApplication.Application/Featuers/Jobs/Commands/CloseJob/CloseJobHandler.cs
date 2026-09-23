using JobApplication.Application.Interfaces;
using JobApplication.Domin.Entities;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace JobApplication.Application.Featuers.Jobs.Commands.CloseJob
{
    public class CloseJobHandler : IRequestHandler<CloseJobCommand, int>
    {
        private readonly IRepository<Job> _jobRepository;

        public CloseJobHandler(IRepository<Job> jobRepository)
        {
            _jobRepository = jobRepository;
        }

        public async Task<int> Handle(CloseJobCommand request, CancellationToken cancellationToken)
        {
            var job = await _jobRepository.GetOneAsync(j => j.Id == request.JobId);

            if (job == null)
            {
                throw new KeyNotFoundException("Job not found.");
            }

            if (job.RecruiterId != request.RecruiterId)
            {
                throw new UnauthorizedAccessException("You are not allowed to close this job.");
            }

            if (!job.IsActive)
            {
                throw new InvalidOperationException("Job is already closed.");
            }

            job.IsActive = false;
            job.ClosedAt = DateTime.UtcNow;
            job.ClosedBy = request.RecruiterId;

            _jobRepository.UpdateAsync(job);
            await _jobRepository.CommitAsync();
            return job.Id ;
        }
    }
}
