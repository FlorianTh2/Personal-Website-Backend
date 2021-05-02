using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;

namespace PersonalWebsiteBackend.Contracts.V1.Responses
{
    // Law #1: Use ISO-8601 for your dates (DateTime.UtcNow.ToString("o");)
    // Law #2: Accept any timezone
    // Law #3: Store it in UTC
    // Law #4: Return it in UTC
    // Law #5: Don’t use time if you don’t need it
    // https://stackoverflow.com/a/37051167
    // http://apiux.com/2013/03/20/5-laws-api-dates-and-times/
    
    [DisplayName("Project")]
    public class ProjectResponse
    {
        public Guid Id { get; set; }

        public string Name { get; set; }
        
        public bool Archived { get; set; }
        
        public string ProjectCreatedOn { get; set; }
        
        public string Description { get; set; }

        public int forksCount { get; set; }

        public string HtmLUrl { get; set; }

        public long ProjectId { get; set; }

        public string Language { get; set; }
        
        public string Licence { get; set; }
        
        public string OwnerName { get; set; }

        public string OwnerHtmlUrl { get; set; }

        public bool Private { get; set; }
        
        public long Size { get; set; }

        public int stars { get; set; }

        public int WatchersCount { get; set; }
        
        public string ProjectUpdatedOn { get; set; }
        
        public string Visibility { get; set; }


        public string UserId { get; set; }
        
        public string CreatedOn { get; set; }

        public string CreatorId { get; set; }

        public string UpdatedOn { get; set; }

        public string UpdaterId { get; set; }
    }
}