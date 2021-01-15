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
        private readonly ITagService _tagService;

        public ProjectService(DataContext dataContext, ITagService tagService)
        {
            _dataContext = dataContext;
            _tagService = tagService;
        }

        public async Task<List<Project>> GetProjectsAsync(GetAllProjectsFilter filter = null, PaginationFilter paginationFilter = null)
        {
            var queryable = _dataContext.Projects.AsQueryable();

            if (paginationFilter == null)
            {
                return await _dataContext.Projects.Include(a => a.Tags).ToListAsync();
            }
            
            queryable = AddFiltersOnQuery(filter, queryable);

            var skip = (paginationFilter.PageNumber - 1) * paginationFilter.PageSize;

            return await _dataContext.Projects.Include(a => a.Tags)
                .Skip(skip)
                .Take(paginationFilter.PageSize)
                .ToListAsync();
        }

        public async Task<Project> GetProjectByIdAsync(Guid projectId)
        {
            return await _dataContext.Projects
                .Include(a => a.Tags)
                .SingleOrDefaultAsync(a => a.Id == projectId);
        }

        // creates assigned tags if they are not already in the database
        public async Task<bool> CreateProjectAsync(Project project)
        {
            project.Tags?.ForEach(x => x.TagName = x.TagName.ToLower());

            await AddNewTags(project); 
            await _dataContext.Projects.AddAsync(project);

            var created = await _dataContext.SaveChangesAsync();
            return created > 0;
        }

        // creates assigned tags if they are not already in the database
        public async Task<bool> UpdateProjectAsync(Project projectToUpdate)
        {
            projectToUpdate.Tags?.ForEach(x=>x.TagName = x.TagName.ToLower());
            await AddNewTags(projectToUpdate);
            _dataContext.Projects.Update(projectToUpdate);
            var updated = await _dataContext.SaveChangesAsync();
            return updated > 0;
        }

        public async Task<bool> DeleteProjectAsync(Guid projectId)
        {
            var project = await GetProjectByIdAsync(projectId);

            if (project == null)
                return false;

            _dataContext.Projects.Remove(project);
            var deleted = await _dataContext.SaveChangesAsync();
            return deleted > 0;
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

        private async Task AddNewTags(Project post)
        {
            foreach (var tag in post.Tags)
            {
                var existingTag =
                    await _dataContext.Tags.SingleOrDefaultAsync(x =>
                        x.Name == tag.TagName);
                if (existingTag != null)
                    continue;

                await _dataContext.Tags.AddAsync(new Tag
                    {Name = tag.TagName, CreatedOn = DateTime.UtcNow, CreatorId = post.UserId});
            }
        }
        
        private static IQueryable<Project> AddFiltersOnQuery(GetAllProjectsFilter filter, IQueryable<Project> queryable)
        {
            if (!string.IsNullOrEmpty(filter?.UserId))
            {
                queryable.Where(a => a.UserId == filter.UserId);
            }

            return queryable;
        }
    }
}