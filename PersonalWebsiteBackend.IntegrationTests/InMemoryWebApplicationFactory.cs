using System;
using System.Linq;
using PersonalWebsiteBackend.Data;
using PersonalWebsiteBackend.Domain;
using PersonalWebsiteBackend.IntegrationTests.Helpers;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace PersonalWebsiteBackend.IntegrationTests
{
    public class InMemoryWebApplicationFactory<TStartup> : WebApplicationFactory<TStartup> where TStartup : class
    {
        // private readonly IConfiguration _configuration;

        public InMemoryWebApplicationFactory()
        {
            
        }
        
        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            // if not specified it is not defined but it runs the TestClient like it is runned in production
            //    (= no dependency on secret-manager -> it reads the environment variables -> thats why i think it runs
            //     like it is in production)
            // builder.UseEnvironment("Production");
            
            // is like Startup-ConfigureServices-Method
            // this builder.ConfigureServices-Method is called AFTER the startups-configureServices-Method
            // because of this we can replace the apps database context here (e.g. with an inmemory one)
            builder.ConfigureServices(async services =>
            {
                var descriptor = services.SingleOrDefault(
                    d => d.ServiceType ==
                         typeof(DbContextOptions<DataContext>));

                services.Remove(descriptor);

                services.AddDbContext<DataContext>(options => { options.UseInMemoryDatabase("InMemoryDbForTesting"); });

                var sp = services.BuildServiceProvider();

                // access in-memory database
                using (var scope = sp.CreateScope())
                {
                    var scopedServices = scope.ServiceProvider;
                    var db = scopedServices.GetRequiredService<DataContext>();
                    var logger = scopedServices
                        .GetRequiredService<ILogger<InMemoryWebApplicationFactory<TStartup>>>();
                    var userManager = scopedServices.GetRequiredService<UserManager<ApplicationUser>>();
                    var roleManager = scopedServices.GetRequiredService<RoleManager<IdentityRole>>();
                    
                    db.Database.EnsureCreated();

                    try
                    {
                        // optionally seed database here
                        await Utilities.InitializeDbForTests(db, userManager, roleManager);
                    }
                    catch (Exception ex)
                    {
                        logger.LogError(ex, "An error occurred seeding the " +
                                            "database with test messages. Error: {Message}", ex.Message);
                    }
                }
            });
        }
    }
}