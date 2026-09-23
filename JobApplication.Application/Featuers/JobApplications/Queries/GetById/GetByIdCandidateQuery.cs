using JobApplication.Domin.Entities;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace JobApplication.Application.Featuers.JobApplications.Queries.GetById
{
    public class GetByIdCandidateQuery : IRequest<Job?>
    {
        public int Id { get; set; }
    }
}
