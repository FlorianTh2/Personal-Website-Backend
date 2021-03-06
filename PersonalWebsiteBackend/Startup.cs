using Hangfire;
using Hangfire.Dashboard;
using PersonalWebsiteBackend.Extensions;
using PersonalWebsiteBackend.Installers;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using PersonalWebsiteBackend.Filter;


namespace PersonalWebsiteBackend
{
    public class Startup
    {
        private IConfiguration Configuration { get; }

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        // framework-function: configure services
        public void ConfigureServices(IServiceCollection services)
        {
            services.InstallProjectServices(Configuration);

            services.InstallDb(Configuration);

            services.InstallMvc(Configuration);

            services.InstallCors();

            services.InstallAutomapper();

            services.InstallHangfire(Configuration);

            services.InstallHangfireJobs();

            services.InstallCacheRedis(Configuration);

            services.InstallSwagger();

            services.InstallHealthCheck();
        }

        // framework-function: configure HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, ILogger<Startup> logger)
        {
            app.UseExceptionHandler("/error");

            app.UseSwagger();

            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "PersonalWebsiteBackend v1");
                if (env.IsDevelopment())
                    c.RoutePrefix = string.Empty;
            });

            // if disabling maybe hangfire will not work since no activity
            // see https://stackoverflow.com/questions/44073911/hangfire-does-not-process-recurring-jobs-unless-dashboard-is-open
            app.UseHangfireDashboard("/hangfire", new DashboardOptions
            {
                IsReadOnlyFunc = (DashboardContext context) => true,
                Authorization = new[] {new HangfireAuthorizationFilter()}
            });

            app.UseCustomHealthChecks();

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseCors();


            // https://www.codeproject.com/Articles/5160941/ASP-NET-CORE-Token-Authentication-and-Authorizatio

            // introduction: https://docs.microsoft.com/de-de/aspnet/core/security/authentication/?view=aspnetcore-5.0
            // -> this authentication middleware uses internally the service: IAuthenticationService
            // -> the authentication-service uses registered authentication-handlers
            // -> The registered authentication handlers and their configuration options are called "schemes".
            // -> in this project the jwt-authentication-scheme is registered in the MvcService (with services.AddAuthentication(/* */)
            // -> this authentication-handler/-scheme contains authentication-related actions
            // -> actions e.g.:
            //      - decrypt the ascii-"encoding" (!=Verschluessellung)
            //      - validate the content of the jwt with the given secret
            //      - Setting the User object in HTTP Request Context
            //      - set the IsAuthenticated 
            app.UseAuthentication();

            app.UseAuthorization();

            app.UseEndpoints(endpoints => { endpoints.MapControllers(); });
        }
    }
}