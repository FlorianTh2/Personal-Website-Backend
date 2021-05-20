using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using PersonalWebsiteBackend.Data;
using PersonalWebsiteBackend.Domain;
using PersonalWebsiteBackend.Extensions;
using PersonalWebsiteBackend.Helpers;
using PersonalWebsiteBackend.Options;

namespace PersonalWebsiteBackend.Services
{
    public class DocumentService : IDocumentService
    {
        private readonly DataContext _dataContext;
        private IConfiguration _configuration;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IDateTimeService _dateTimeService;

        public DocumentService(DataContext dataContext, IConfiguration configuration, UserManager<ApplicationUser> userManager, IDateTimeService dateTimeService)
        {
            _dataContext = dataContext;
            _configuration = configuration;
            _userManager = userManager;
            _dateTimeService = dateTimeService;
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
                Documents = await queryable.AsNoTracking().OrderByDescending(a=>a.DocumentCreatedTime).Skip(skip).Take(paginationFilter.PageSize).ToListAsync(),
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
        
        public async Task UpdateDocumentsInDatabaseAsync()
        {
            var seedAdminProfile = new SeedAdminProfile();
            _configuration.Bind(nameof(seedAdminProfile), seedAdminProfile);
            var user = await _userManager.FindByEmailAsync(seedAdminProfile.Email);

            var googleDriveAPISettings = new GoogleDriveAPISettings();
            _configuration.Bind(nameof(googleDriveAPISettings), googleDriveAPISettings);

            var files = await FetchGoogleDriveDocuments.FetchAllDocumentsWithAllProperties(googleDriveAPISettings);
            
            var allDocumentsOfUser = _dataContext.Documents.ToDictionary(a => a.DocumentId);
            
            foreach (var file in files)
            {
                // the api also sends the folder as a file in the result, but i want only the "real" files
                if (file.Size != null)
                {
                    var document = file.ConvertToDocument();
                    document.UserId = user.Id;
                    
                    if (allDocumentsOfUser.ContainsKey(document.DocumentId))
                    {
                        document.Id = allDocumentsOfUser[document.DocumentId].Id;
                        document.CreatorId = "SYSTEM";
                        document.UpdaterId = "SYSTEM";
                        document.CreatedOn = _dateTimeService.Now;
                        document.UpdatedOn = _dateTimeService.Now;
                        _dataContext.Entry(allDocumentsOfUser[document.DocumentId]).CurrentValues.SetValues(document);
                    }
                    else
                    {
                        document.CreatorId = "SYSTEM";
                        document.UpdaterId = "SYSTEM";
                        document.CreatedOn = _dateTimeService.Now;
                        document.UpdatedOn = _dateTimeService.Now;
                        await _dataContext.AddAsync(document);
                    }
                }
            }
            // delete document in database if it is not in drive-folder
            foreach (KeyValuePair<string, Document> entry in allDocumentsOfUser)
            {
                if (!files.Where(a => a.Id == entry.Key).Any())
                {
                    _dataContext.Documents.Remove(entry.Value);
                }
            }
            await _dataContext.SaveChangesAsync();
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