﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using AutoMapper;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Drive.v3;
using Google.Apis.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Octokit;
using PersonalWebsiteBackend.Domain;
using PersonalWebsiteBackend.Extensions;
using PersonalWebsiteBackend.Helpers;
using PersonalWebsiteBackend.Options;
using Project = PersonalWebsiteBackend.Domain.Project;

namespace PersonalWebsiteBackend.Data
{
    public static class DataContextSeed
    {
        public static async Task SeedDefaultUserAsync(UserManager<ApplicationUser> userManager,
            RoleManager<IdentityRole> roleManager, IConfiguration config)
        {
            var seedAdminProfile = new SeedAdminProfile();
            config.Bind(nameof(seedAdminProfile), seedAdminProfile);

            var administratorRole = new IdentityRole(seedAdminProfile.IdentityRoleName);

            if (roleManager.Roles.All(r => r.Name != administratorRole.Name))
            {
                await roleManager.CreateAsync(administratorRole);
            }

            var administrator = new ApplicationUser()
            {
                Email = seedAdminProfile.Email,
                UserName = seedAdminProfile.Email
            };

            if (userManager.Users.All(u => u.Email != administrator.Email))
            {
                await userManager.CreateAsync(administrator, seedAdminProfile.Password);
                await userManager.AddToRolesAsync(administrator, new[] {administratorRole.Name});
            }
        }

        public static async Task SeedProjectDataAsync(UserManager<ApplicationUser> userManager, DataContext context,
            IConfiguration config)
        {
            if (!context.Projects.Any())
            {
                // get admin
                var seedAdminProfile = new SeedAdminProfile();
                config.Bind(nameof(seedAdminProfile), seedAdminProfile);
                var user = await userManager.FindByEmailAsync(seedAdminProfile.Email);

                var githubSettings = new GithubSettings();
                config.Bind(nameof(githubSettings), githubSettings);

                var repositoryUser = await FetchGithubRepositories.FetchCurrentUser(githubSettings);
                var repositories =
                    await FetchGithubRepositories.FetchAllGithubRepositoriesForCurrentUser(githubSettings);
                
                foreach (Repository repository in repositories)
                {
                    // login == login-name
                    if (repository.Owner.Login == repositoryUser.Login)
                    {
                        Project project = repository.ConvertToProject();
                        project.UserId = user.Id;
                        await context.AddAsync(project);
                    }
                }

                var created = await context.SaveChangesAsync();
                if (created == 0)
                    throw new Exception("Error: No database entries got generated by seeding.");
            }
        }

        public static async Task SeedDocumentsDataAsync(UserManager<ApplicationUser> userManager, DataContext context,
            IConfiguration config)
        {
            if (!context.Documents.Any())
            {
                var seedAdminProfile = new SeedAdminProfile();
                config.Bind(nameof(seedAdminProfile), seedAdminProfile);
                var user = await userManager.FindByEmailAsync(seedAdminProfile.Email);

                var googleDriveAPISettings = new GoogleDriveAPISettings();
                config.Bind(nameof(googleDriveAPISettings), googleDriveAPISettings);

                var files = await FetchGoogleDriveDocuments.FetchAllDocumentsWithAllProperties(googleDriveAPISettings);
                foreach (var file in files)
                {
                    // the api also sends the folder as a file in the result, but i want only the "real" files
                    if (file.Size != null)
                    {
                        var document = file.ConvertToDocument();
                        document.UserId = user.Id;
                        await context.AddAsync(document);
                    }
                }

                var created = await context.SaveChangesAsync();
                if (created == 0)
                    throw new Exception("Error: No database entries got generated by seeding.");
            }
        }
    }
}