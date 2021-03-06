﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Octokit;
using PersonalWebsiteBackend.Data;
using PersonalWebsiteBackend.Domain;
using PersonalWebsiteBackend.Extensions;
using PersonalWebsiteBackend.Helpers;
using PersonalWebsiteBackend.Options;
using Project = PersonalWebsiteBackend.Domain.Project;

namespace PersonalWebsiteBackend.Services
{
    public class ProjectService : IProjectService
    {
        private readonly DataContext _dataContext;
        private readonly IConfiguration _configuration;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IDateTimeService _dateTimeService;

        public ProjectService(DataContext dataContext, IConfiguration configuration,
            UserManager<ApplicationUser> userManager, IDateTimeService dateTimeService)
        {
            _dataContext = dataContext;
            _configuration = configuration;
            _userManager = userManager;
            _dateTimeService = dateTimeService;
        }

        public async Task<GetProjectsAsyncServiceResponse> GetProjectsAsync(GetAllProjectsFilter filter = null,
            PaginationFilter paginationFilter = null)
        {
            var queryable = _dataContext.Projects.Include(a => a.User).AsQueryable();

            queryable = AddFiltersOnQuery(filter, queryable, this._configuration);

            var skip = (paginationFilter.PageNumber - 1) * paginationFilter.PageSize;

            var serviceResponse = new GetProjectsAsyncServiceResponse()
            {
                Projects = await queryable.AsNoTracking().OrderByDescending(a=>a.ProjectCreatedOn).Skip(skip).Take(paginationFilter.PageSize).ToListAsync(),
                TotalProjects = await queryable.AsNoTracking().LongCountAsync()
            };

            return serviceResponse;
        }

        public async Task<Project> GetProjectByIdAsync(Guid projectId)
        {
            return await _dataContext.Projects
                .SingleOrDefaultAsync(a => a.Id == projectId);
        }

        public async Task<bool> UserOwnsProjectAsync(Guid projectId, string userId)
        {
            var project = await _dataContext.Projects.AsNoTracking().SingleOrDefaultAsync(a => a.Id == projectId);

            if (project == null)
            {
                return false;
            }

            if (project.UserId != userId)
            {
                return false;
            }

            return true;
        }

        private static IQueryable<Project> AddFiltersOnQuery(GetAllProjectsFilter filter, IQueryable<Project> queryable,
            IConfiguration config)
        {
            if (filter == null || string.IsNullOrEmpty(filter?.UserId))
            {
                // if no filter is provided: return projects of admin
                var seedAdminProfile = new SeedAdminProfile();
                config.Bind(nameof(seedAdminProfile), seedAdminProfile);
                queryable = queryable.Where(a => a.User.Email == seedAdminProfile.Email);
            }
            else
            {
                queryable = queryable.Where(a => a.UserId == filter.UserId);
            }

            return queryable;
        }

        public async Task UpdateProjectsInDatabaseAsync()
        {
            // get admin
            var seedAdminProfile = new SeedAdminProfile();
            _configuration.Bind(nameof(seedAdminProfile), seedAdminProfile);
            var user = await _userManager.FindByEmailAsync(seedAdminProfile.Email);

            var githubSettings = new GithubSettings();
            _configuration.Bind(nameof(githubSettings), githubSettings);

            var repositoryUser = await FetchGithubRepositories.FetchCurrentUser(githubSettings);
            var repositories =
                await FetchGithubRepositories.FetchAllGithubRepositoriesForCurrentUser(githubSettings);

            var allProjectOfUser = _dataContext.Projects.ToDictionary(a => a.ProjectId);

            foreach (Repository repository in repositories)
            {
                // login == login-name
                if (repository.Owner.Login == repositoryUser.Login)
                {
                    Project project = repository.ConvertToProject();
                    project.UserId = user.Id;
                    
                    if (allProjectOfUser.ContainsKey(project.ProjectId))
                    {
                        project.Id = allProjectOfUser[project.ProjectId].Id;
                        project.CreatorId = "SYSTEM";
                        project.UpdaterId = "SYSTEM";
                        project.CreatedOn = _dateTimeService.Now;
                        project.UpdatedOn = _dateTimeService.Now;
                        _dataContext.Entry(allProjectOfUser[project.ProjectId]).CurrentValues.SetValues(project);
                    }
                    else
                    {
                        project.CreatorId = "SYSTEM";
                        project.UpdaterId = "SYSTEM";
                        project.CreatedOn = _dateTimeService.Now;
                        project.UpdatedOn = _dateTimeService.Now;
                        await _dataContext.AddAsync(project);
                    }
                }
            }
            foreach (KeyValuePair<long, Project> entry in allProjectOfUser)
            {
                if (!repositories.Where(a => a.Id == entry.Key).Any())
                {
                    _dataContext.Projects.Remove(entry.Value);
                }
            }
            await _dataContext.SaveChangesAsync();
        }
    }
}