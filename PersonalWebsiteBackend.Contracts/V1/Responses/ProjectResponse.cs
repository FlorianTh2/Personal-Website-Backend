using System;
using System.Collections;
using System.Collections.Generic;

namespace PersonalWebsiteBackend.Contracts.V1.Responses
{
    public class ProjectResponse
    {
        public Guid Id { get; set; }

        public string Name { get; set; }

        public string UserId { get; set; }
        
        public DateTime CreatedOn { get; set; }

        public string CreatorId { get; set; }

        public DateTime UpdatedOn { get; set; }

        public string UpdaterId { get; set; }
    }
}