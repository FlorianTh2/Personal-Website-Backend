using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using PersonalWebsiteBackend.Contracts.V1;
using PersonalWebsiteBackend.Contracts.V1.Requests;
using PersonalWebsiteBackend.Contracts.V1.Responses;
using PersonalWebsiteBackend.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PersonalWebsiteBackend.Cache;
using PersonalWebsiteBackend.Domain;
using PersonalWebsiteBackend.Services;

namespace PersonalWebsiteBackend.Controllers.V1
{
    public class DocumentController : Controller
    {
        private readonly IDocumentService _documentService;
        private readonly IUriService _uriService;
        private readonly IMapper _mapper;

        public DocumentController(IDocumentService documentService, IUriService uriService, IMapper mapper)
        {
            _documentService = documentService;
            _uriService = uriService;
            _mapper = mapper;
        }

        [HttpGet]
        [Route(ApiRoutes.Documents.GetAll)]
        [Cache(600)]
        public async Task<IActionResult> GetAll()
        {
            var documents = await _documentService.GetDocumentsAsync();
            return Ok(new Response<List<DocumentResponse>>(_mapper.Map<List<DocumentResponse>>(documents)));
        }

        [HttpGet]
        [Route(ApiRoutes.Documents.Get)]
        [Cache(600)]
        public async Task<IActionResult> Get([FromRoute] Guid documentId)
        {
            var document = await _documentService.GetDocumentByIdAsync(documentId);

            if (document == null)
                return NotFound();

            return Ok(new Response<DocumentResponse>(_mapper.Map<DocumentResponse>(document)));
        }
    }
}