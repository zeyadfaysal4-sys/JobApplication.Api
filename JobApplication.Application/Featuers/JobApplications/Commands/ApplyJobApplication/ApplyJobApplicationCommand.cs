using MediatR;

namespace JobApplication.Application.Featuers.JobApplications.Commands.ApplyJobApplication
{
    public class ApplyJobApplicationCommand : IRequest
    {
        public int JobId { get; set; }
        public string ApplicationUserId { get; set; }
    }
}
