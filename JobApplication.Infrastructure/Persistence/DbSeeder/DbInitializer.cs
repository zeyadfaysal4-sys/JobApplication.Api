using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;


namespace JobApplication.Infrastructure.Persistence.DbSeeder
{
    public class DbInitializer : IDbInitializer
    {
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly ILogger<DbInitializer> _logger;

        public DbInitializer(
            RoleManager<IdentityRole> roleManager,
            ILogger<DbInitializer> logger)
        {
            _roleManager = roleManager;
            _logger = logger;
        }

        public async Task InitializeAsync()
        {
            try
            {
                var roles = new[]
                {
                    Roles.Recruiter,
                    Roles.Candidate
                };

                foreach (var role in roles)
                {
                    if (!await _roleManager.RoleExistsAsync(role))
                    {
                        var result = await _roleManager.CreateAsync(new IdentityRole(role));

                        if (!result.Succeeded)
                        {
                            foreach (var error in result.Errors)
                            {
                                _logger.LogError("Error creating role {Role}: {Error}",role,error.Description);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while seeding roles.");
            }
        }
    }
}