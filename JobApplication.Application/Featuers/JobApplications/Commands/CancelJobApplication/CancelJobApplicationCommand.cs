using MediatR;

namespace JobApplication.Application.Featuers.JobApplications.Commands.CancelJobApplication
{
    public class CancelJobApplicationCommand : IRequest
    {
        public int ApplicationId { get; set; }
        public string ApplicationUserId { get; set; }
    }
}
