using JobApplication.Application.Interfaces;
using JobApplication.Domin.Entities;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace JobApplication.Application.Featuers.Jobs.Queries.GetAllJob
{
    public class GetAllJobHandler : IRequestHandler<GetAllJobQuery, IEnumerable<Job>>
    {
        private readonly IRepository<Job> _jobRepository;

        public GetAllJobHandler(IRepository<Job> jobRepository)
        {
            _jobRepository = jobRepository;
        }

        public async Task<IEnumerable<Job>> Handle(GetAllJobQuery request, CancellationToken cancellationToken)
        {
            var jobs = await _jobRepository.GetAsync();
            return jobs;
        }
    }
}
