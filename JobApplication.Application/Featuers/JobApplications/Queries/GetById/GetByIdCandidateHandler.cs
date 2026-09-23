using JobApplication.Application.Interfaces;
using JobApplication.Domin.Entities;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace JobApplication.Application.Featuers.JobApplications.Queries.GetById
{
    public class GetByIdCandidateHandler : IRequestHandler<GetByIdCandidateQuery, Job?>
    {
        private readonly IRepository<Job> _jobRepository;

        public GetByIdCandidateHandler(IRepository<Job> jobRepository)
        {
            _jobRepository = jobRepository;
        }

        public async Task<Job?> Handle(GetByIdCandidateQuery request, CancellationToken cancellationToken)
        {
            var job = await _jobRepository.GetOneAsync(j => j.Id == request.Id, tracked: false);
            return job;
        }
    }
}
