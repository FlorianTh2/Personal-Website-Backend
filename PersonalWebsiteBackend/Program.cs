using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using PersonalWebsiteBackend.Options;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using PersonalWebsiteBackend.Data;
using PersonalWebsiteBackend.Domain;
using Serilog;

namespace PersonalWebsiteBackend
{
    public class Program
    {
        // framework-function: creates + configure host-builder object
        public static async Task Main(string[] args)
        {
            var host = CreateHostBuilder(args).Build();

            // migrate database schema + seed database
            using (var serviceScope = host.Services.CreateScope())
            {
                var dbContext = serviceScope.ServiceProvider.GetRequiredService<DataContext>();
                await dbContext.Database.MigrateAsync();

                var env = serviceScope.ServiceProvider.GetRequiredService<IWebHostEnvironment>();
                var userManager = serviceScope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
                var roleManager = serviceScope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>(); 
                IConfiguration config = GetConfiguration(env);

                await DataContextSeed.SeedDefaultUserAsync(userManager, roleManager, config);
                await DataContextSeed.SeedProjectDataAsync(userManager, dbContext, config);
                await DataContextSeed.SeedDocumentsDataAsync(userManager, dbContext, config);
            }

            await host.RunAsync();
        }
        
        
        // framework-function: builds + runs methods on host-builder object (CreateDefaultBuilder == e.g. injects default logger, load appsettings.json)
        // create
        public static IHostBuilder CreateHostBuilder(string[] args)
        {
            return Host
                .CreateDefaultBuilder(args)
                .UseSerilog((context, configuration) =>
                {
                    configuration
                        .Enrich.FromLogContext()
                        .Enrich.WithMachineName()
                        .WriteTo.Console()
                        .Enrich.WithProperty("Environment", context.HostingEnvironment.EnvironmentName)
                        .ReadFrom.Configuration(context.Configuration);
                })
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });
        }


        // helper function
        // add secrets for database seeding
        private static IConfiguration GetConfiguration(IWebHostEnvironment env)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddEnvironmentVariables();
            return builder.Build();
        }
    }
}