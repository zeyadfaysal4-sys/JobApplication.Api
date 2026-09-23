using MediatR;


namespace JobApplication.Application.Featuers.Jobs.Commands.OpenJob
{
    public class OpenJobCommand : IRequest<int>
    {
        public int JobId { get; set; }
        public string RecruiterId { get; set; }
    }
}
