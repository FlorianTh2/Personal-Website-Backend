using Microsoft.Extensions.DependencyInjection;
using PersonalWebsiteBackend.Data;
using PersonalWebsiteBackend.HealthChecks;

namespace PersonalWebsiteBackend.Installers
{
    public static class HealthChecksInstaller
    {
        public static void InstallHealthCheck(this IServiceCollection services)
        {
            services.AddHealthChecks()
                .AddDbContextCheck<DataContext>()
                .AddCheck<RedisHealthCheck>("redis");
        }
    }
}