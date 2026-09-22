using JobApplication.Application.DTO;
using JobApplication.Domin.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace JobApplication.Application.Interfaces
{
    public interface IjobService
    {
        Task<int> CreateJob(CreateJobDto createJobDto, string recruiterId);

        Task CloseJob(int jobId, string recruiterId);

         Task<IEnumerable<Job>> GetAllJob();

         Task<Job?> GetByIdJob(int id);
    }
}
