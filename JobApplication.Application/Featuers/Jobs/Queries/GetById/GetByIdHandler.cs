using JobApplication.Application.Interfaces;
using JobApplication.Domin.Entities;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace JobApplication.Application.Featuers.Jobs.Queries.GetById
{
    public class GetByIdHandler : IRequestHandler<GetByIdQuery, Job?>
    {
        private readonly IRepository<Job> _jobRepository;

        public GetByIdHandler(IRepository<Job> jobRepository)
        {
            _jobRepository = jobRepository;
        }

        public async Task<Job?> Handle(GetByIdQuery request, CancellationToken cancellationToken)
        {
            var job = await _jobRepository.GetOneAsync(j => j.Id == request.Id, tracked: false);
            return job;
        }
    }
}
