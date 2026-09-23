using JobApplication.Domin.Entities;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace JobApplication.Application.Featuers.JobApplication.Queries.GetAllJob
{
    public class GetAllJobQuery : IRequest<IEnumerable<Job>>
    {

    }
}
