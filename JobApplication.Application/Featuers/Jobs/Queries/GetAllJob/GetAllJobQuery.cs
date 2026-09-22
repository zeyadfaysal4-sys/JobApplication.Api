using JobApplication.Domin.Entities;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace JobApplication.Application.Featuers.Jobs.Queries.GetAllJob
{
    public class GetAllJobQuery : IRequest<IEnumerable<Job>>
    {

    }
}
