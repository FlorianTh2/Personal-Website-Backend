using System;
using Hangfire;
using Hangfire.PostgreSql;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using PersonalWebsiteBackend.Data;

namespace PersonalWebsiteBackend.Installers
{
    public static class HangfireInstaller
    {
        public static void InstallHangfire(this IServiceCollection services, IConfiguration configuration)
        {
            string connectionString = configuration.GetConnectionString("HelloHangfireConnectionString");
            
            services.AddDbContext<HangfireDbContext>(options => 
                options.UseNpgsql(connectionString));
            
            // Add Hangfire services.
            services.AddHangfire(config => config
                .SetDataCompatibilityLevel(CompatibilityLevel.Version_170)
                .UseSimpleAssemblyNameTypeSerializer()
                .UseRecommendedSerializerSettings()
                .UsePostgreSqlStorage(connectionString,
                    new PostgreSqlStorageOptions()
                    {
                        QueuePollInterval = TimeSpan.FromSeconds(15),
                        InvisibilityTimeout = TimeSpan.FromMinutes(30.0),
                        DistributedLockTimeout = TimeSpan.FromMinutes(10.0),
                        TransactionSynchronisationTimeout = TimeSpan.FromMilliseconds(500.0),
                        UseNativeDatabaseTransactions = true,
                        PrepareSchemaIfNecessary = true,
                    })
            );
            
            

            // Add the processing server as IHostedService
            services.AddHangfireServer(options =>
            {
                // specifies in which intervall server checks for possible enqueued jobs
                options.SchedulePollingInterval = TimeSpan.FromSeconds(1);
                options.HeartbeatInterval = TimeSpan.FromSeconds(30.0);
            });
        }
    }
}