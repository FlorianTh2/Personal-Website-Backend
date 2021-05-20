using Microsoft.Extensions.DependencyInjection;
using PersonalWebsiteBackend.Services;

namespace PersonalWebsiteBackend.Installers
{
    public static class HangfireJobsInstaller
    {
        public static void InstallHangfireJobs(this IServiceCollection services)
        {
            // services.AddHostedService<RecurringJobsService>();
        }
    }
}