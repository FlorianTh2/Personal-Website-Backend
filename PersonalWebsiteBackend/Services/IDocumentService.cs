using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using PersonalWebsiteBackend.Domain;

namespace PersonalWebsiteBackend.Services
{
    public interface IDocumentService
    {
        Task<GetDocumentsAsyncServiceResponse> GetDocumentsAsync(GetAllDocumentsFilter filter = null, PaginationFilter paginationFilter = null);

        Task<Document> GetDocumentByIdAsync(Guid documentId);
        
        Task<bool> UserOwnsDocumentAsync(Guid documentId, string userId);
    }
}