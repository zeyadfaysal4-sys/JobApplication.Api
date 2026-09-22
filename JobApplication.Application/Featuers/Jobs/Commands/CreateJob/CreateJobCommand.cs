using MediatR;

namespace JobApplication.Application.Featuers.Jobs.Commands.CreateJob
{
    public class CreateJobCommand : IRequest<int>
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public bool IsActive { get; set; }

        public string RecruiterId { get; set; }
    }
}
