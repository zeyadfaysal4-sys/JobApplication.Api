using JobApplication.Application.Interfaces;
using JobApplication.Domin.Entities;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace JobApplication.Application.Featuers.Jobs.Commands.OpenJob
{
    public class OpenJobHandler : IRequestHandler<OpenJobCommand, int>
    {
        private readonly IRepository<Job> _jobRepository;

        public OpenJobHandler(IRepository<Job> jobRepository)
        {
            _jobRepository = jobRepository;
        }

        async Task<int> IRequestHandler<OpenJobCommand, int>.Handle(OpenJobCommand request, CancellationToken cancellationToken)
        {
            var job = await _jobRepository.GetOneAsync(j => j.Id == request.JobId);

            if (job == null)
            {
                throw new KeyNotFoundException("Job not found.");
            }

            if (job.RecruiterId != request.RecruiterId)
            {
                throw new UnauthorizedAccessException("You are not allowed to open this job.");
            }

            if (job.IsActive)
            {
                throw new InvalidOperationException("Job is already opened.");
            }

            job.IsActive = true;
            job.OpenedAt = DateTime.UtcNow;
            job.OpenedBy = request.RecruiterId;

            _jobRepository.UpdateAsync(job);
            await _jobRepository.CommitAsync();
            return job.Id;
        }
    }
}
