using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using PersonalWebsiteBackend.Contracts.V1.Requests;
using PersonalWebsiteBackend.Contracts.V1.Responses;
using Refit;

namespace PersonalWebsiteBackend.Sdk
{
    [Headers("Authorization: Bearer")]
    public interface IPersonalWebsiteBackendApi
    {
        [Get("/api/v1/projects")]
        Task<ApiResponse<List<ProjectResponse>>> GetAllAsync();
        
        [Get("/api/v1/projects/{projectId}")]
        Task<ApiResponse<ProjectResponse>> GetAsync(Guid projectId);
        
        [Post("/api/v1/projects")]
        Task<ApiResponse<ProjectResponse>> CreateAsync([Body] CreateProjectRequest createProjectRequest);
        
        [Put("/api/v1/projects/{projectId}")]
        Task<ApiResponse<ProjectResponse>> UpdateAsync(Guid projectId, [Body] UpdateProjectRequest updateProjectRequest);
        
        [Delete("/api/v1/projects/{projectId}")]
        Task<ApiResponse<string>> DeleteAsync(Guid projectId);
    }
}