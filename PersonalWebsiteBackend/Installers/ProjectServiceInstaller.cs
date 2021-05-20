using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using PersonalWebsiteBackend.Services;

namespace PersonalWebsiteBackend.Installers
{
    public static class ProjectServiceInstaller
    {
        public static void InstallProjectServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddSingleton<ICurrentUserService, CurrentUserService>();

            services.AddTransient<IDateTimeService, DateTimeServiceService>();


            // changed to scoped because of tracking?!
            // services.AddSingleton<IProjectService, ProjectService>();
            services.AddScoped<IProjectService, ProjectService>();

            services.AddScoped<IDocumentService, DocumentService>();

            services.AddSingleton<IConfiguration>(configuration);

            services.AddTransient<IMessageService, MessageService>();
        }
    }
}