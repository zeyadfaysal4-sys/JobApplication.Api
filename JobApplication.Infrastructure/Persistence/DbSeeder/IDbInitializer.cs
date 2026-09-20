using System;
using System.Collections.Generic;
using System.Text;

namespace JobApplication.Infrastructure.Persistence.DbSeeder
{
    public interface IDbInitializer
    {
        Task InitializeAsync();
    }
}
