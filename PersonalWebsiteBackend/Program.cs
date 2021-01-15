using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using PersonalWebsiteBackend.Options;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using PersonalWebsiteBackend.Data;
using PersonalWebsiteBackend.Domain;

namespace PersonalWebsiteBackend
{
    // used secretManager
    // local_file_path: C:\Users\Florian\AppData\Roaming\Microsoft\UserSecrets\01d49ea2-0701-4783-8ef5-6471a1c32caa\secrets.json
    public class Program
    {
        // dotnet run --launch-settings "dev"
        //
        // docker build -t PersonalWebsiteBackend .
        // docker run -p 5000:5000 PersonalWebsiteBackend
        //
        // docker-compose build
        // docker-compose up
        // https://0.0.0.0:5000/swagger/index.html
        // docker-compose down

        public static async Task Main(string[] args)
        {
            var host = CreateHostBuilder(args).Build();

            using (var serviceScope = host.Services.CreateScope())
            {
                var dbContext = serviceScope.ServiceProvider.GetRequiredService<DataContext>();
                await dbContext.Database.MigrateAsync();

                var env = serviceScope.ServiceProvider.GetRequiredService<IWebHostEnvironment>();
                var userManager = serviceScope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
                var roleManager = serviceScope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
                IConfiguration config = GetConfiguration(env);

                await DataContextSeed.SeedDefaultUserAsync(userManager, roleManager, config);
            }

            await host.RunAsync();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder => { webBuilder.UseStartup<Startup>(); });


        private static IConfiguration GetConfiguration(IWebHostEnvironment env)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddEnvironmentVariables();

            if (env.IsDevelopment())
            {
                builder.AddUserSecrets<Startup>();
            }


            return builder.Build();
        }
    }
}