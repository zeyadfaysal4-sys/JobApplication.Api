using JobApplication.Application.DTO;
using JobApplication.Domin.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace JobApplication.Application.Interfaces
{
    public interface IjobService
    {
        Task<int> Create(CreateJobDto createJobDto, string recruiterId);

        Task Close(int jobId, string recruiterId);

         Task<IEnumerable<Job>> GetAll();

         Task<Job?> GetById(int id);
    }
}
