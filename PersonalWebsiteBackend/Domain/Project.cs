using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Octokit;

namespace PersonalWebsiteBackend.Domain
{
    public class Project : AuditableEntity
    {
        [Key] public Guid Id { get; set; }

        public string Name { get; set; }
        
        public bool Archived { get; set; }
        
        public DateTime ProjectCreatedOn { get; set; }
        
        public string Description { get; set; }

        public int forksCount { get; set; }

        public string HtmLUrl { get; set; }

        public long ProjectId { get; set; }

        public string Language { get; set; }
        
        public string Licence { get; set; }
        
        public string OwnerName { get; set; }

        public string OwnerHTMLURL { get; set; }

        public bool Private { get; set; }
        
        public long Size { get; set; }

        public int stars { get; set; }

        public int WatchersCount { get; set; }
        
        public DateTime ProjectUpdatedOn { get; set; }

        [Column(TypeName = "varchar(24)")]
        public Visibility Visibility { get; set; }
        
        public string UserId { get; set; }

        [ForeignKey(nameof(UserId))]
        public ApplicationUser User { get; set; }
    }
}