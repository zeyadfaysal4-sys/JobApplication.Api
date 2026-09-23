using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace JobApplication.Application.Featuers.Jobs.Commands.CloseJob
{
    public class CloseJobCommand : IRequest<int>
    {
        public int JobId { get; set; }
        public string RecruiterId { get; set; }
    }
}
