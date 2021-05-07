using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using PersonalWebsiteBackend.Domain;

namespace PersonalWebsiteBackend.Services
{
    public interface IProjectService
    {
        Task<GetProjectsAsyncServiceResponse> GetProjectsAsync(GetAllProjectsFilter filter = null, PaginationFilter paginationFilter = null);

        Task<Project> GetProjectByIdAsync(Guid projectId);
        
        Task<bool> UserOwnsProjectAsync(Guid projectId, string userId);
    }
}