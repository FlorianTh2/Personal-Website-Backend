using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using PersonalWebsiteBackend.Data;
using PersonalWebsiteBackend.Domain;
using PersonalWebsiteBackend.Options;

namespace PersonalWebsiteBackend.Services
{
    public class ProjectService : IProjectService
    {
        private readonly DataContext _dataContext;
        private readonly IConfiguration _configuration;

        public ProjectService(DataContext dataContext, IConfiguration configuration)
        {
            _dataContext = dataContext;
            _configuration = configuration;
        }

        public async Task<GetProjectsAsyncServiceResponse> GetProjectsAsync(GetAllProjectsFilter filter = null,
            PaginationFilter paginationFilter = null)
        {
            var queryable = _dataContext.Projects.Include(a => a.User).AsQueryable();

            queryable = AddFiltersOnQuery(filter, queryable, this._configuration);

            var skip = (paginationFilter.PageNumber - 1) * paginationFilter.PageSize;

            var serviceResponse = new GetProjectsAsyncServiceResponse()
            {
                Projects = await queryable.AsNoTracking().Skip(skip).Take(paginationFilter.PageSize).ToListAsync(),
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
    }
}