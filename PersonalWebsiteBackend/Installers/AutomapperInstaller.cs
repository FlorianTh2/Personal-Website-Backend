using System;
using AutoMapper;
using Microsoft.Extensions.DependencyInjection;

namespace PersonalWebsiteBackend.Installers
{
    public static class AutomapperInstaller
    {
        public static void InstallAutomapper(this IServiceCollection services)
        {
            services.AddAutoMapper(config =>
            {
                config.CreateMap<DateTime, string>().ConvertUsing(dt => dt.ToString("o"));
            }, typeof(Startup));
        }
    }
}