﻿using System;
using PersonalWebsiteBackend.Contracts.V1.Requests.Queries;

namespace PersonalWebsiteBackend.Services
{
    public interface IUriService
    {
        Uri GetUri(string apiRoute, string id);
        
        Uri GetAllUri(string apiRoute, PaginationQuery pagination = null);
    }
}