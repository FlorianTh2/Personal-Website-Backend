using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using PersonalWebsiteBackend.Contracts.V1;
using PersonalWebsiteBackend.Contracts.V1.Requests;
using PersonalWebsiteBackend.Contracts.V1.Requests.Queries;
using PersonalWebsiteBackend.Contracts.V1.Responses;
using PersonalWebsiteBackend.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PersonalWebsiteBackend.Cache;
using PersonalWebsiteBackend.Domain;
using PersonalWebsiteBackend.Helpers;
using PersonalWebsiteBackend.Services;

namespace PersonalWebsiteBackend.Controllers.V1
{
    public class ProjectsController : Controller
    {
        private readonly IProjectService _projectService;
        private readonly IMapper _mapper;
        private readonly IUriService _uriService;

        public ProjectsController(IProjectService projectService, IMapper mapper, IUriService uriService)
        {
            _projectService = projectService;
            _mapper = mapper;
            _uriService = uriService;
        }

        [HttpGet]
        [Route(ApiRoutes.Projects.GetAll)]
        [Cache(600)] 
        public async Task<IActionResult> GetAll([FromQuery] GetAllProjectsQuery query, [FromQuery]PaginationQuery paginationQuery)
        {
            var paginationFilter = _mapper.Map<PaginationFilter>(paginationQuery);
            var filter = _mapper.Map<GetAllProjectsFilter>(query);
            
            var projects = await _projectService.GetProjectsAsync(filter, paginationFilter);
            var projectsResponse = _mapper.Map<List<ProjectResponse>>(projects);

            if (paginationFilter == null || paginationFilter.PageNumber < 1 || paginationFilter.PageSize < 1)
            {
                return Ok(new PagedResponse<ProjectResponse>(projectsResponse));
            }
            
            return Ok(PaginationHelpers.CreatePaginatedResponse(_uriService, paginationFilter, projectsResponse));
        }

        [HttpGet]
        [Route(ApiRoutes.Projects.Get)]
        public async Task<IActionResult> Get([FromRoute] Guid projectId)
        {
            var project = await _projectService.GetProjectByIdAsync(projectId);

            if (project == null)
                NotFound();

            return Ok(new Response<ProjectResponse>(_mapper.Map<ProjectResponse>(project)));
        }
    }
}