using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using PersonalWebsiteBackend.Domain;

namespace PersonalWebsiteBackend.Services
{
    public interface IDocumentService
    {
        Task<List<Document>> GetDocumentsAsync();

        Task<Document> GetDocumentByIdAsync(Guid documentId);
        
        Task<bool> UserOwnsDocumentAsync(Guid documentId, string userId);
    }
}