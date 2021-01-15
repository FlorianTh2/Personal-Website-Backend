using System;
using System.Collections.Generic;

namespace PersonalWebsiteBackend.Contracts.V1.Requests
{
    public class CreateProjectRequest
    {
        public string Name { get; set; }
        
        public IEnumerable<string> Tags { get; set; }

    }
}