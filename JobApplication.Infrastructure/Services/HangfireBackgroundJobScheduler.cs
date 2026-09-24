using Hangfire;
using JobApplication.Application.Interfaces;
using System.Linq.Expressions;

namespace JobApplication.Infrastructure.Services
{
    public class HangfireBackgroundJobScheduler : IBackgroundJobScheduler
    {
        private readonly IBackgroundJobClient _backgroundJobClient;

        public HangfireBackgroundJobScheduler(IBackgroundJobClient backgroundJobClient)
        {
            _backgroundJobClient =backgroundJobClient;
        }

        public void Enqueue<T>(Expression<Action<T>> methodCall)
        {
            _backgroundJobClient.Enqueue<T>( methodCall);
        }

        public void Schedule<T>(Expression<Action<T>> methodCall,TimeSpan delay)
        {
            _backgroundJobClient.Schedule<T>(methodCall,delay);
        }
    }
}
