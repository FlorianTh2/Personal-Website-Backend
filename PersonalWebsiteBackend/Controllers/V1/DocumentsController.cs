using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using PersonalWebsiteBackend.Contracts.V1;
using PersonalWebsiteBackend.Contracts.V1.Responses;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using PersonalWebsiteBackend.Cache;
using PersonalWebsiteBackend.Contracts.V1.Requests.Queries;
using PersonalWebsiteBackend.Domain;
using PersonalWebsiteBackend.Helpers;
using PersonalWebsiteBackend.Services;

namespace PersonalWebsiteBackend.Controllers.V1
{
    [Produces("application/json")]
    [ApiController]
    public class DocumentController : ControllerBase
    {
        private readonly IDocumentService _documentService;
        private readonly IUriService _uriService;
        private readonly IMapper _mapper;
        private readonly ILogger<DocumentController> _logger;

        public DocumentController(IDocumentService documentService, IUriService uriService, IMapper mapper, ILogger<DocumentController> logger)
        {
            _documentService = documentService;
            _uriService = uriService;
            _mapper = mapper;
            _logger = logger;
        }

        
        /// <summary>
        /// Get all Documents
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route(ApiRoutes.Documents.GetAll, Name = "[controller]_[action]")]
        [Cache(600)]
        public async Task<ActionResult<PagedResponse<DocumentResponse>>> GetAll([FromQuery] GetAllDocumentsQuery query, [FromQuery] PaginationQuery paginationQuery)
        {
            var paginationFilter = _mapper.Map<PaginationFilter>(paginationQuery);
            var filter = _mapper.Map<GetAllDocumentsFilter>(query);

            var serviceResponse = await _documentService.GetDocumentsAsync(filter, paginationFilter);
            var documentsResponse = _mapper.Map<List<DocumentResponse>>(serviceResponse.Documents);
            
            if (paginationFilter == null || paginationFilter.PageNumber < 1 || paginationFilter.PageSize < 1)
            {
                return Ok(new PagedResponse<DocumentResponse>(documentsResponse));
            }

            return Ok(PaginationHelpers.CreatePaginatedResponse(_uriService,ApiRoutes.Documents.GetAll, paginationFilter, documentsResponse, serviceResponse.TotalDocuments ));
        }

        /// <summary>
        /// Get one document by its documentId
        /// </summary>
        /// <param name="documentId"></param>
        /// <returns></returns>
        [HttpGet]
        [Route(ApiRoutes.Documents.Get, Name = "[controller]_[action]")]
        [Cache(600)]
        public async Task<ActionResult<Response<DocumentResponse>>> Get([FromRoute] Guid documentId)
        {
            var document = await _documentService.GetDocumentByIdAsync(documentId);

            if (document == null)
                return NotFound();

            return Ok(new Response<DocumentResponse>(_mapper.Map<DocumentResponse>(document)));
        }
    }
}