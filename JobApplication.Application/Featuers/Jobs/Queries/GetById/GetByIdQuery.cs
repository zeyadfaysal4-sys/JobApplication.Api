using JobApplication.Domin.Entities;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace JobApplication.Application.Featuers.Jobs.Queries.GetById
{
    public class GetByIdQuery : IRequest<Job?>
    {
        public int Id { get; set; }
    }
}
