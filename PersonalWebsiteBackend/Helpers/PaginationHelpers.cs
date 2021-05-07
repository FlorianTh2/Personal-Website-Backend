using System.Collections.Generic;
using System.Linq;
using PersonalWebsiteBackend.Contracts.V1.Requests.Queries;
using PersonalWebsiteBackend.Contracts.V1.Responses;
using PersonalWebsiteBackend.Domain;
using PersonalWebsiteBackend.Services;

namespace PersonalWebsiteBackend.Helpers
{
    public class PaginationHelpers
    {
        public static PagedResponse<T> CreatePaginatedResponse<T>(
            IUriService uriService,
            string apiRoute,
            PaginationFilter paginationFilter,
            List<T> response,
            long totalCount
        )
        {
            // caluclate next/previous page
            var nextPage = paginationFilter.PageNumber >= 1
                ? uriService
                    .GetAllUri(apiRoute,  new PaginationQuery(paginationFilter.PageNumber + 1, paginationFilter.PageSize))
                    .ToString()
                : null;

            var previousPage = paginationFilter.PageNumber - 1 >= 1
                ? uriService
                    .GetAllUri(apiRoute, new PaginationQuery(paginationFilter.PageNumber - 1, paginationFilter.PageSize))
                    .ToString()
                : null;

            // mapping from domain to the contract
            return new PagedResponse<T>()
            {
                Data = response,
                PageNumber = paginationFilter.PageNumber >= 1 ? paginationFilter.PageNumber : (int?) null,
                PageSize = paginationFilter.PageSize >= 1 ? paginationFilter.PageSize : (int?) null,
                NextPage = (response.Any() && paginationFilter.PageNumber * paginationFilter.PageSize < totalCount) ? nextPage : null,
                PreviousPage = previousPage,
                ItemsTotal = (int)totalCount,
                PagesTotal = (int)((totalCount + paginationFilter.PageSize - 1) / paginationFilter.PageSize),
            };
        }
    }
}