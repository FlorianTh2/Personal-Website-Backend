using System;
using PersonalWebsiteBackend.Data;
using PersonalWebsiteBackend.Domain;
using PersonalWebsiteBackend.Extensions;
using PersonalWebsiteBackend.Installers;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;

namespace PersonalWebsiteBackend
{
    public class Startup
    {
        public IConfiguration Configuration { get; }

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }
        
        // framework-function: configure services
        public void ConfigureServices(IServiceCollection services)
        {
            services.InstallDb(Configuration);

            services.InstallMvc(Configuration);
            
            services.InstallAutomapper();

            services.InstallCacheRedis(Configuration);

            services.InstallSwagger();

            services.InstallHealthCheck();
        }

        // framework-function: configure HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, ILogger<Startup> logger)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                // app.UseExceptionHandler();
            }
            
            app.UseSwagger();
            
            app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "PersonalWebsiteBackend v1"));
            
            app.UseCustomHealthChecks();
            
            app.UseHttpsRedirection();
            
            app.UseRouting();


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