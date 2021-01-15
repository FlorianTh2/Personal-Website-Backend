using System;
using System.IO;
using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.Filters;

namespace PersonalWebsiteBackend.Installers
{
    public static class SwaggerInstaller
    {
        public static void InstallSwagger(this IServiceCollection services)
        {
            services.AddSwaggerGen(a =>
            {
                a.SwaggerDoc("v1", new OpenApiInfo
                {
                    Version = "v1",
                    Title = "PersonalWebsiteBackend",
                    Description = "PersonalWebsiteBackend API Swagger Surface",
                    Contact = new OpenApiContact
                    {
                        Name = "Florian Thom",
                        Email = "thom.florian@yahoo.de",
                    },
                });
                
                // make swagger load the SwaggerExamples/ - folder
                a.ExampleFilters();
                
                a.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Name = "Authorization",
                    Description = "JWT Authorization header using the Bearer scheme (Example: 'Bearer 12345abcdef')",
                    Scheme = "Bearer",
                    BearerFormat = "JWT",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.ApiKey,
                });

                a.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "Bearer"
                            }
                        },
                        Array.Empty<string>()
                    }
                });

                // loads swaggers-xml-file and loads it into the middleware
                // to make swagger to generate that file, we had to adjust the .csproj-file
                // and add some stuff there
                // actually the file is a documentation file generate from asp.net/the C# project
                // and has less to do with swagger
                // the xml-file is generated to the /bin folder
                var xmlFile = Assembly.GetExecutingAssembly().GetName().Name.ToString() + ".xml";
                var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                a.IncludeXmlComments(xmlPath);
            });

            // make swagger load the SwaggerExamples/ - folder
            services.AddSwaggerExamplesFromAssemblyOf<Startup>();
        }
    }
}