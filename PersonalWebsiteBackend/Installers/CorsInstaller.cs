using System;
using System.Text;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using PersonalWebsiteBackend.Filter;
using PersonalWebsiteBackend.Options;
using PersonalWebsiteBackend.Services;

namespace PersonalWebsiteBackend.Installers
{
    public static class CorsInstaller
    {
        public static void InstallCors(this IServiceCollection services)
        {
            services.AddCors(options =>
            {
                options.AddDefaultPolicy(builder =>
                    builder.WithOrigins("http://localhost:3000", "https://localhost:3000", "http://florianthom.io", "https://florianthom.io", "http://www.florianthom.io", "https://www.florianthom.io")
                        .AllowAnyHeader()
                        .AllowAnyMethod()
                );
            });
        }
    }
}