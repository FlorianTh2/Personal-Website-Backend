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
    public static class MvcInstaller
    {
        public static void InstallMvc(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddControllers(options =>
                {
                    options.Filters.Add<ValidationFilter>();
                })
                .AddFluentValidation(configuration =>
                    configuration.RegisterValidatorsFromAssemblyContaining<Startup>());

            var jwtSettings = new JwtSettings();
            configuration.Bind(nameof(jwtSettings), jwtSettings);
            services.AddSingleton(jwtSettings);

            services.AddScoped<IIdentityService, IdentityService>();

            var tokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(jwtSettings.Secret)),
                ValidateIssuer = false,
                ValidateAudience = false,
                RequireExpirationTime = false,
                ValidateLifetime = true,
                ClockSkew = TimeSpan.Zero
            };

            services.AddSingleton(tokenValidationParameters);

            // will attempt to create a user identity and make it available for the framework
            // enables to store the access-token in the HTTPContext.User - property
            // helpful to get the claims from that later in the controller
            // introduction: https://docs.microsoft.com/de-de/aspnet/core/security/authentication/?view=aspnetcore-5.0
            services.AddAuthentication(a =>
                {
                    // because of this setting: the [Authorize]-Attribute in the Controller
                    // dont need to set [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)] anymore
                    // since we set here the default
                    //
                    // what are these schemes?: https://stackoverflow.com/a/52493428/11244995
                    a.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                    a.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
                    a.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                })
                .AddJwtBearer(b =>
                {
                    b.SaveToken = true;
                    b.TokenValidationParameters = tokenValidationParameters;
                });

            // in detail: github.com/FlorianTh2/dotnet5BackendProject
            services.AddAuthorization();
            
            services.AddSingleton<IUriService>(provider =>
            {
                var request = provider.GetRequiredService<IHttpContextAccessor>()
                    .HttpContext?
                    .Request;
                var absoluteUri = string.Concat(request.Scheme, "://", request.Host.ToUriComponent(), "/");
                return new UriService(absoluteUri);
            });

            services.AddSingleton<IConfiguration>(configuration);
        }
    }
}