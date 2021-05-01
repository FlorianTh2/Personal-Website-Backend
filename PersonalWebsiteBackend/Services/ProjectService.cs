using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using PersonalWebsiteBackend.Data;
using PersonalWebsiteBackend.Domain;

namespace PersonalWebsiteBackend.Services
{
    public class ProjectService : IProjectService
    {
        private readonly DataContext _dataContext;
        
        public ProjectService(DataContext dataContext)
        {
            _dataContext = dataContext;
        }

        public async Task<List<Project>> GetProjectsAsync(GetAllProjectsFilter filter = null, PaginationFilter paginationFilter = null)
        {
            var queryable = _dataContext.Projects.AsQueryable();

            if (paginationFilter == null)
            {
                return new List<Project>();
            }
            
            queryable = AddFiltersOnQuery(filter, queryable);

            var skip = (paginationFilter.PageNumber - 1) * paginationFilter.PageSize;

            return await queryable
                .Skip(skip)
                .Take(paginationFilter.PageSize)
                .ToListAsync();
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
        
        private static IQueryable<Project> AddFiltersOnQuery(GetAllProjectsFilter filter, IQueryable<Project> queryable)
        {
            if (!string.IsNullOrEmpty(filter?.UserId))
            {
                Console.WriteLine(filter.UserId);
                queryable = queryable.Where(a => a.UserId == filter.UserId);
            }

            return queryable;
        }
    }
}