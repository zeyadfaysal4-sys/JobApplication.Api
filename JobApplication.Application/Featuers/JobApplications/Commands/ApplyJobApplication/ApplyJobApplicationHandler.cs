using JobApplication.Application.Interfaces;
using JobApplication.Domin.Entities;
using JobApplication.Domin.Enums;
using MediatR;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace JobApplication.Application.Featuers.JobApplications.Commands.ApplyJobApplication
{
    public class ApplyJobApplicationHandler : IRequestHandler<ApplyJobApplicationCommand>
    {
        private readonly IRepository<Domin.Entities.JobApplication> _jobApplicationRepository;
        private readonly IRepository<Domin.Entities.Candidate> _candidateRepository;
        private readonly IRepository<Domin.Entities.Job> _jobRepository;

        public ApplyJobApplicationHandler(
            IRepository<Domin.Entities.JobApplication> jobApplicationRepository,
            IRepository<Domin.Entities.Candidate> candidateRepository,
            IRepository<Domin.Entities.Job> jobRepository)
        {
            _jobApplicationRepository = jobApplicationRepository;
            _candidateRepository = candidateRepository;
            _jobRepository = jobRepository;
        }

        public async Task Handle(ApplyJobApplicationCommand request, CancellationToken cancellationToken)
        {
            var candidate = await _candidateRepository.GetOneAsync(
                c => c.ApplicationUserId == request.ApplicationUserId);

            if (candidate == null)
            {
                throw new KeyNotFoundException("Candidate not found.");
            }

            var job = await _jobRepository.GetOneAsync(j => j.Id == request.JobId);

            if (job == null)
            {
                throw new KeyNotFoundException("Job not found.");
            }

            if (!job.IsActive)
            {
                throw new InvalidOperationException("You cannot apply for a closed job.");
            }

            var existingApplication = await _jobApplicationRepository.GetOneAsync(
                a => a.JobId == request.JobId && a.CandidateId == candidate.Id);

            if (existingApplication != null)
            {
                throw new InvalidOperationException("You have already applied for this job.");
            }

            var application = new Domin.Entities.JobApplication
            {
                JobId = request.JobId,
                CandidateId = candidate.Id,
                JobApplicationStatus = JobApplicationStatus.Applied,
                AppliedAt = DateTime.UtcNow,
                StatusUpdatedAt = DateTime.UtcNow
            };

            await _jobApplicationRepository.AddAsync(application);
            await _jobApplicationRepository.CommitAsync();
        }
    }
}
