using System;
using PersonalWebsiteBackend.Contracts.V1.Requests.Queries;

namespace PersonalWebsiteBackend.Services
{
    public interface IUriService
    {
        Uri GetProjectUri(string projectId);

        Uri GetAllProjectsUri(PaginationQuery pagination = null);
        
        Uri GetDocumentUri(string documentId);

        Uri GetAllDocumentsUri(PaginationQuery pagination = null);
    }
}