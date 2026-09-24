using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace JobApplication.Application.Interfaces
{
    public interface IBackgroundJobScheduler
    {
        void Enqueue<T>(Expression<Action<T>> methodCall);

        void Schedule<T>(
            Expression<Action<T>> methodCall,
            TimeSpan delay);
    }
}
