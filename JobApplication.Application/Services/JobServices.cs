using JobApplication.Application.DTO;
using JobApplication.Application.Interfaces;
using JobApplication.Domin.Entities;

namespace JobApplication.Application.Services
{
    public class JobServices
    {
        private readonly IRepository<Job> _jobRepository;

        public JobServices(IRepository<Job> jobRepository)
        {
            _jobRepository = jobRepository;
        }

        public async Task<int> CreateJob(CreateJobDto createJobDto,string recruiterId)
        {
            var job = new Job()
            {
                Title = createJobDto.Title,
                Description = createJobDto.Description,
                IsActive = createJobDto.IsActive,
                RecruiterId = recruiterId
            };

            await _jobRepository.AddAsync(job);
            await _jobRepository.CommitAsync();

            return job.Id;
        }

        public async Task CloseJob(int jobId, string recruiterId)
        {
            var job = await _jobRepository.GetOneAsync(j => j.Id == jobId);

            if (job == null)
            {
                throw new KeyNotFoundException("Job not found.");
            }

            if (job.RecruiterId != recruiterId)
            {
                throw new UnauthorizedAccessException("You are not allowed to close this job.");
            }

            if (!job.IsActive)
            {
                throw new InvalidOperationException("Job is already closed.");
            }

            job.IsActive = false;
            job.ClosedAt = DateTime.UtcNow;
            job.ClosedBy = recruiterId;

            await _jobRepository.CommitAsync();
        }

        public async Task<IEnumerable<Job>> GetAllJob()
        {
            return await _jobRepository.GetAsync(tracked: false);
        }

        public async Task<Job?> GetByIdJob(int id)
        {
            return await _jobRepository.GetOneAsync(j => j.Id == id, tracked: false);
        }
    }
}