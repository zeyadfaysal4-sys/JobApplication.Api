using JobApplication.Application.Interfaces;
using JobApplication.Domin.Entities;
using Microsoft.Extensions.Logging;

namespace JobApplication.Infrastructure.Services
{
    public class JobExpirationService : IJobExpirationService
    {
        private readonly IRepository<Job> _jobRepository;
        private readonly ILogger<JobExpirationService> _logger;

        public JobExpirationService(
            IRepository<Job> jobRepository,ILogger<JobExpirationService> logger)
        {
            _jobRepository = jobRepository;
            _logger = logger;
        }

        public async Task CheckExpiredJobs()
        {
            var jobs = await _jobRepository.GetAsync(j => j.IsActive && j.ExpiresAt.HasValue && j.ExpiresAt.Value <= DateTime.UtcNow);

            foreach (var job in jobs)
            {
                job.IsActive = false;
                job.ClosedAt = DateTime.UtcNow;
                job.ClosedBy = "System";

                _logger.LogInformation("Job {JobId} was automatically closed because it expired.", job.Id);
            }

            await _jobRepository.CommitAsync();
        }
    }
}