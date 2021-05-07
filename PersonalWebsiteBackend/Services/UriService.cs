﻿using System;
 using System.Text.RegularExpressions;
 using PersonalWebsiteBackend.Contracts.V1;
using PersonalWebsiteBackend.Contracts.V1.Requests.Queries;
using Microsoft.AspNetCore.WebUtilities;

namespace PersonalWebsiteBackend.Services
{
    // you can also add request.path to the baseUri at the installer so you dont need to add the api routes here
    // but when your baseUri evolves to a "path"-uri
    public class UriService : IUriService
    {
        private readonly string _baseUri;

        public UriService(string baseUri)
        {
            _baseUri = baseUri;
        }

        public Uri GetUri(string apiRoute, string id)
        {
            return new Uri(_baseUri + Regex.Replace(apiRoute, "{.*?}", id));
        }

        public Uri GetAllUri(string apiRoute,PaginationQuery pagination = null)
        {
            var uri = new Uri(_baseUri + apiRoute);

            if (pagination == null)
            {
                return uri;
            }
            
            var modifiedUri = QueryHelpers.AddQueryString(uri.ToString(), "pageNumber", pagination.PageNumber.ToString());
            modifiedUri = QueryHelpers.AddQueryString(modifiedUri, "pageSize", pagination.PageSize.ToString());
            return new Uri(modifiedUri);
        }
    }
}