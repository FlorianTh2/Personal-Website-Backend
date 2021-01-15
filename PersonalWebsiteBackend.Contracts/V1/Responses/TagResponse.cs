using System;

namespace PersonalWebsiteBackend.Contracts.V1.Responses
{
    public class TagResponse
    {
        public string Name { get; set; }
        
        public DateTime CreatedOn { get; set; }

        public string CreatorId { get; set; }

        public DateTime? UpdatedOn { get; set; }

        public string UpdaterId { get; set; }
    }
}