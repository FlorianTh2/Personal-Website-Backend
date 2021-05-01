using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using PersonalWebsiteBackend.Contracts.V1;
using PersonalWebsiteBackend.Contracts.V1.Responses;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using PersonalWebsiteBackend.Cache;
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
        public async Task<ActionResult<Response<List<DocumentResponse>>>> GetAll()
        {
            var documents = await _documentService.GetDocumentsAsync();
            return Ok(new Response<List<DocumentResponse>>(_mapper.Map<List<DocumentResponse>>(documents)));
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