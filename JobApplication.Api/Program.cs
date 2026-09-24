using JobApplication.Application.Interfaces;
using JobApplication.Domin.Entities;
using JobApplication.Infrastructure.Persistence;
using JobApplication.Infrastructure.Persistence.DbSeeder;
using JobApplication.Infrastructure.Repositories;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Scalar.AspNetCore;
using Hangfire;
using Hangfire.SqlServer;
using JobApplication.Infrastructure.Services;
using System.Text;

namespace JobApplication.Api
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddControllers();

            builder.Services.AddCors(options =>
            {
                options.AddPolicy("AllowScalar", policy =>
                {
                    policy.AllowAnyOrigin()
                          .AllowAnyHeader()
                          .AllowAnyMethod();
                });
            });

            var connectionString =
                builder.Configuration.GetConnectionString("DefaultConnection")
                ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

            var hangfireConnection =
                builder.Configuration.GetConnectionString("HangfireConnection")
                ?? throw new InvalidOperationException("Connection string 'HangfireConnection' not found.");

            builder.Services.AddDbContext<ApplicationDbContext>(options =>
            {
                options.UseSqlServer(connectionString);
            });

            builder.Services
                .AddIdentity<ApplicationUser, IdentityRole>()
                .AddEntityFrameworkStores<ApplicationDbContext>()
                .AddDefaultTokenProviders();

            var jwtSettings = builder.Configuration.GetSection("JwtSettings");

            var securityKey =
                jwtSettings["securityKey"] ?? throw new InvalidOperationException("JWT security key not found.");

            builder.Services
                .AddAuthentication(options =>
                {
                    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;

                    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                })
                .AddJwtBearer(options =>
                {
                    options.TokenValidationParameters =
                        new TokenValidationParameters
                        {
                            ValidateIssuer = true,
                            ValidateAudience = true,
                            ValidateLifetime = true,
                            ValidateIssuerSigningKey = true,

                            ValidIssuer = jwtSettings["validIssuer"],
                            ValidAudience = jwtSettings["validAudience"],

                            IssuerSigningKey =
                                new SymmetricSecurityKey(Encoding.UTF8.GetBytes(securityKey))
                        };
                });

          
            builder.Services.AddScoped<IBackgroundJobScheduler, HangfireBackgroundJobScheduler>();
            builder.Services.AddScoped<INotificationService, EmailNotificationService>();
            builder.Services.AddScoped<IJobExpirationService, JobExpirationService>();
            builder.Services.AddScoped<IRepository<Job>, Repository<Job>>();
            builder.Services.AddScoped<IRepository<Candidate>, Repository<Candidate>>();
            builder.Services.AddScoped<IRepository<Domin.Entities.JobApplication>, Repository<Domin.Entities.JobApplication>>();
            builder.Services.AddScoped<IDbInitializer, DbInitializer>();

            builder.Services.AddHangfire(config =>
            {
                config
                    .UseSimpleAssemblyNameTypeSerializer()
                    .UseRecommendedSerializerSettings()
                    .UseSqlServerStorage(hangfireConnection);
            });

            builder.Services.AddHangfireServer();

            builder.Services.AddOpenApi();
            builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(JobApplication.Application.Featuers.Jobs.Commands.CreateJob.CreateJobCommand).Assembly));



            var app = builder.Build();

            using (var scope = app.Services.CreateScope())
            {
                var recurringJobManager =
                    scope.ServiceProvider.GetRequiredService<IRecurringJobManager>();

                recurringJobManager.AddOrUpdate<IJobExpirationService>(
                    "check-expired-jobs",
                    service => service.CheckExpiredJobs(),
                    Cron.Minutely);
            }

            using (var scope = app.Services.CreateScope())
            {
                var initializer = scope.ServiceProvider.GetRequiredService<IDbInitializer>();
                await initializer.InitializeAsync();
            }
                
            if (app.Environment.IsDevelopment())
            {
                app.MapOpenApi();
                app.MapScalarApiReference();
            }

            app.UseHttpsRedirection();

            app.UseCors("AllowScalar");

            app.UseAuthentication();
            app.UseAuthorization();
            app.UseHangfireDashboard("/hangfire");

            app.MapControllers();

            await app.RunAsync();
        }
    }
}