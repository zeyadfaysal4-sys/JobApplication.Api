using JobApplication.Application.Interfaces;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;

namespace JobApplication.Infrastructure.Services
{
    public class EmailNotificationService : INotificationService
    {
        private readonly IRepository<Domin.Entities.JobApplication>_jobApplicationRepository;

        private readonly ILogger<EmailNotificationService> _logger;

        public EmailNotificationService(IRepository<Domin.Entities.JobApplication>jobApplicationRepository,ILogger<EmailNotificationService> logger)
        {
            _jobApplicationRepository =jobApplicationRepository;

            _logger = logger;
        }

        public void NotifyRecruiter(int applicationId)
        {
            var application = _jobApplicationRepository.GetOneAsync(a => a.Id == applicationId).GetAwaiter().GetResult();

            if (application == null)
            {
                _logger.LogWarning("Application {ApplicationId} was not found.",applicationId);
                return;
            }

            _logger.LogInformation(
                "Send Email: Candidate {CandidateId} applied to Job {JobId}. ApplicationId: {ApplicationId}",
                application.CandidateId,
                application.JobId,
                applicationId);
        }
    }
}
