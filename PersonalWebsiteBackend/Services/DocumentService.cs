using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using PersonalWebsiteBackend.Data;
using PersonalWebsiteBackend.Domain;
using PersonalWebsiteBackend.Options;

namespace PersonalWebsiteBackend.Services
{
    public class DocumentService : IDocumentService
    {
        private readonly DataContext _dataContext;
        private IConfiguration _configuration;

        public DocumentService(DataContext dataContext, IConfiguration configuration)
        {
            _dataContext = dataContext;
            _configuration = configuration;
        }

        public async Task<GetDocumentsAsyncServiceResponse> GetDocumentsAsync(GetAllDocumentsFilter filter = null,
            PaginationFilter paginationFilter = null)
        {
            var queryable = _dataContext.Documents.Include(a => a.User).AsQueryable();
            var skip = (paginationFilter.PageNumber - 1) * paginationFilter.PageSize;
            
            queryable = AddFiltersOnQuery(filter, queryable, this._configuration);
            
            var serviceResponse = new GetDocumentsAsyncServiceResponse()
            {
                TotalDocuments = await queryable.AsNoTracking().LongCountAsync(),
                Documents = await queryable.AsNoTracking().Skip(skip).Take(paginationFilter.PageSize).ToListAsync(),
            };
            return serviceResponse;
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
        
        private static IQueryable<Document> AddFiltersOnQuery(GetAllDocumentsFilter filter, IQueryable<Document> queryable,
            IConfiguration config)
        {
            if (filter == null || string.IsNullOrEmpty(filter?.UserId))
            {
                // if no filter is provided: return projects of admin
                var seedAdminProfile = new SeedAdminProfile();
                config.Bind(nameof(seedAdminProfile), seedAdminProfile);
                queryable = queryable.Where(a => a.User.Email == seedAdminProfile.Email);
            }
            else
            {
                queryable = queryable.Where(a => a.UserId == filter.UserId);
            }

            return queryable;
        }
    }
}