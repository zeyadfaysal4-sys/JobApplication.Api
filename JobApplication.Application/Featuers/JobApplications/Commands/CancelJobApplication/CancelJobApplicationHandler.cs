using JobApplication.Application.Interfaces;
using JobApplication.Domin.Entities;
using JobApplication.Domin.Enums;
using MediatR;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace JobApplication.Application.Featuers.JobApplications.Commands.CancelJobApplication
{
    public class CancelJobApplicationHandler : IRequestHandler<CancelJobApplicationCommand>
    {
        private readonly IRepository<Domin.Entities.JobApplication> _jobApplicationRepository;
        private readonly IRepository<Domin.Entities.Candidate> _candidateRepository;

        public CancelJobApplicationHandler(
            IRepository<Domin.Entities.JobApplication> jobApplicationRepository,
            IRepository<Domin.Entities.Candidate> candidateRepository)
        {
            _jobApplicationRepository = jobApplicationRepository;
            _candidateRepository = candidateRepository;
        }

        public async Task Handle(CancelJobApplicationCommand request, CancellationToken cancellationToken)
        {
            var candidate = await _candidateRepository.GetOneAsync(
                c => c.ApplicationUserId == request.ApplicationUserId);

            if (candidate == null)
            {
                throw new KeyNotFoundException("Candidate not found.");
            }

            var application = await _jobApplicationRepository.GetOneAsync(
                a => a.Id == request.ApplicationId);

            if (application == null)
            {
                throw new KeyNotFoundException("Application not found.");
            }

            if (application.CandidateId != candidate.Id)
            {
                throw new UnauthorizedAccessException("You are not allowed to cancel this application.");
            }

            if (application.JobApplicationStatus != JobApplicationStatus.Applied &&
                application.JobApplicationStatus != JobApplicationStatus.UnderReview)
            {
                throw new InvalidOperationException("Application cannot be cancelled in its current status.");
            }

            application.JobApplicationStatus = JobApplicationStatus.Cancelled;
            application.CancelledAt = DateTime.UtcNow;
            application.StatusUpdatedAt = DateTime.UtcNow;

            await _jobApplicationRepository.CommitAsync();
        }
    }
}
