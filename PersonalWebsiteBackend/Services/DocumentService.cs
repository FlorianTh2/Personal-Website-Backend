using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using PersonalWebsiteBackend.Data;
using PersonalWebsiteBackend.Domain;

namespace PersonalWebsiteBackend.Services
{
    public class DocumentService : IDocumentService
    {
        private readonly DataContext _dataContext;

        public DocumentService(DataContext dataContext)
        {
            _dataContext = dataContext;
        }

        public async Task<List<Document>> GetDocumentsAsync()
        { 
            return await _dataContext.Documents.AsNoTracking().ToListAsync();
        }

        public async Task<Document> GetDocumentByIdAsync(Guid documentId)
        {
            return await _dataContext.Documents.SingleOrDefaultAsync(b => b.Id == documentId);
        }
        
        public async Task<bool> UserOwnsDocumentAsync(Guid documentId, string userId)
        {
            var project = await _dataContext.Documents.AsNoTracking().SingleOrDefaultAsync(a => a.Id == documentId);

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
    }
}