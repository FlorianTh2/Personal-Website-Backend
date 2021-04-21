﻿using System;
using PersonalWebsiteBackend.Contracts.V1.Requests.Queries;

namespace PersonalWebsiteBackend.Services
{
    public interface IUriService
    {
        Uri GetAllProjectsUri(PaginationQuery pagination = null);
        
        Uri GetAllDocumentsUri(PaginationQuery pagination = null);
    }
}