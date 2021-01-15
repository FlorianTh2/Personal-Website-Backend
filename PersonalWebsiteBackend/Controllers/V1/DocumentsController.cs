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
    [Authorize]
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

        [HttpPost]
        [Route(ApiRoutes.Documents.Create)]
        public async Task<IActionResult> Create([FromBody] CreateDocumentRequest documentRequest)
        {
            var newDocumentId = Guid.NewGuid();
            var document = new Document()
            {
                Id = newDocumentId,
                Name = documentRequest.Name,
                Topic = documentRequest.Topic,
                UserId = HttpContext.GetUserId(),
                
                Description = documentRequest.Description,
                Link = documentRequest.Link,
                Tags = documentRequest.Tags.Select(a => new DocumentTag()
                {
                    DocumentId = newDocumentId,
                    TagName = a
                }).ToList(),
            };

            await _documentService.CreateDocumentAsync(document);
            var locationUri = _uriService.GetDocumentUri(document.Id.ToString());
            return Created(locationUri, new Response<DocumentResponse>(_mapper.Map<DocumentResponse>(document)));
        }

        [HttpPut]
        [Route(ApiRoutes.Documents.Update)]
        public async Task<IActionResult> Update([FromRoute] Guid documentId, [FromBody] UpdateDocumentRequest documentRequest)
        {
            bool userOwnsDocument = await _documentService.UserOwnsDocumentAsync(documentId, HttpContext.GetUserId());

            if (!userOwnsDocument)
            {
                return BadRequest(error: "You dont own this document");
            }

            Document document = await _documentService.GetDocumentByIdAsync(documentId);
            
            document.Name = documentRequest.Name;
            document.Description = documentRequest.Description;
            document.Topic = documentRequest.Topic;
            document.Link = documentRequest.Link;
            
            
            bool updated = await _documentService.UpdateDocumentAsync(document);

            if (updated)
            {
                return Ok(new Response<DocumentResponse>(_mapper.Map<DocumentResponse>(document)));
            }

            return NotFound();

        }

        [HttpDelete]
        [Route(ApiRoutes.Documents.Delete)]
        public async Task<IActionResult> Delete(Guid documentId)
        {
            bool userOwnsDocument = await _documentService.UserOwnsDocumentAsync(documentId, HttpContext.GetUserId());

            if (!userOwnsDocument)
            {
                return BadRequest(error: "You dont own this document");
            }

            bool deleted = await _documentService.DeleteDocumentAsync(documentId);

            if (deleted)
            {
                return NoContent();
            }

            return NotFound();
        }
        
        
    }
}