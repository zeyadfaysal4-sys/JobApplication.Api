using System;
using System.Collections.Generic;
using System.Text;

namespace JobApplication.Application.Interfaces
{
    public interface IJobExpirationService
    {
        Task CheckExpiredJobs();
    }
}
