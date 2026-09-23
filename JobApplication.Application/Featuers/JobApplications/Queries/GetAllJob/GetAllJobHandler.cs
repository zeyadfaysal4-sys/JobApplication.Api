using JobApplication.Application.Interfaces;
using JobApplication.Domin.Entities;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace JobApplication.Application.Featuers.JobApplication.Queries.GetAllJob
{
    public class GetAllJobHandler : IRequestHandler<GetAllJobQuery, IEnumerable<Job>>
    {
        private readonly IRepository<Job> _job;

        public GetAllJobHandler(IRepository<Job> job)
        {
            _job = job;
        }

        public async Task<IEnumerable<Job>> Handle(GetAllJobQuery request, CancellationToken cancellationToken)
        {
            var jobs = await _job.GetAsync();

            return jobs;
        }
    }
}
