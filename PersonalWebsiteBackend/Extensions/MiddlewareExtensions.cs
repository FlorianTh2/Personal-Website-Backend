using System.Linq;
using System.Text.Json.Serialization;
using PersonalWebsiteBackend.Contracts.HealthChecks;
using PersonalWebsiteBackend.Data;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;

namespace PersonalWebsiteBackend.Extensions
{
    public static class MiddlewareExtensions
    {
        public static void UseCustomHealthChecks(this IApplicationBuilder app)
        {
            app.UseHealthChecks("/health", new HealthCheckOptions()
            {
                ResponseWriter = async (context, report) =>
                {
                    context.Response.ContentType = "application/json";
                    
                    var response = new HealthCheckResponse()
                    {
                        Status = report.Status.ToString(),
                        Checks = report.Entries.Select(a => new HealthCheck()
                        {
                            Component = a.Key,
                            Status = a.Value.Status.ToString(),
                            Description = a.Value.Description
                        }),
                        Duration = report.TotalDuration
                    };

                    await context.Response.WriteAsync(JsonConvert.SerializeObject(response));
                }
            });
        }
    }
}