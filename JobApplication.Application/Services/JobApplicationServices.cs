using JobApplication.Application.Interfaces;
using JobApplication.Domin.Entities;
using JobApplication.Domin.Enums;

namespace JobApplication.Application.Services
{
    public class JobApplicationServices
    {
        private readonly IRepository<Domin.Entities.JobApplication> _jobApplicationRepository;
        private readonly IRepository<Domin.Entities.Candidate> _candidateRepository;
        private readonly IRepository<Domin.Entities.Job> _jobRepository;

        public JobApplicationServices(
            IRepository<Domin.Entities.JobApplication> jobApplicationRepository,
            IRepository<Domin.Entities.Candidate> candidateRepository,
            IRepository<Domin.Entities.Job> jobRepository)
        {
            _jobApplicationRepository = jobApplicationRepository;
            _candidateRepository = candidateRepository;
            _jobRepository = jobRepository;
        }

        public async Task Apply(int jobId,string applicationUserId)
        {
            var candidate =
                await _candidateRepository.GetOneAsync(
                    c => c.ApplicationUserId == applicationUserId);

            if (candidate == null)
            {
                throw new KeyNotFoundException(
                    "Candidate not found.");
            }

            var job =
                await _jobRepository.GetOneAsync(
                    j => j.Id == jobId);

            if (job == null)
            {
                throw new KeyNotFoundException(
                    "Job not found.");
            }

            if (!job.IsActive)
            {
                throw new InvalidOperationException(
                    "You cannot apply for a closed job.");
            }

            var existingApplication =
                await _jobApplicationRepository.GetOneAsync(
                    a =>
                        a.JobId == jobId &&
                        a.CandidateId == candidate.Id);

            if (existingApplication != null)
            {
                throw new InvalidOperationException(
                    "You have already applied for this job.");
            }

            var application =
                new Domin.Entities.JobApplication
                {
                    JobId = jobId,
                    CandidateId = candidate.Id,
                    JobApplicationStatus =
                        JobApplicationStatus.Applied,
                    AppliedAt = DateTime.UtcNow,
                    StatusUpdatedAt = DateTime.UtcNow
                };

            await _jobApplicationRepository.AddAsync(application);

            await _jobApplicationRepository.CommitAsync();
        }

        public async Task Cancel(int applicationId,string applicationUserId)
        {
            var candidate =await _candidateRepository.GetOneAsync( c => c.ApplicationUserId == applicationUserId);

            if (candidate == null)
            {
                throw new KeyNotFoundException("Candidate not found.");
            }

            var application =await _jobApplicationRepository.GetOneAsync(a => a.Id == applicationId);

            if (application == null)
            {
                throw new KeyNotFoundException("Application not found.");
            }

            if (application.CandidateId != candidate.Id)
            {
                throw new UnauthorizedAccessException("You are not allowed to cancel this application.");
            }

            if (application.JobApplicationStatus !=
                    JobApplicationStatus.Applied && application.JobApplicationStatus !=JobApplicationStatus.UnderReview)
            {
                throw new InvalidOperationException("Application cannot be cancelled in its current status.");
            }

            application.JobApplicationStatus =JobApplicationStatus.Cancelled;

            application.CancelledAt =DateTime.UtcNow;

            application.StatusUpdatedAt =DateTime.UtcNow;

            await _jobApplicationRepository.CommitAsync();
        }
    }
}