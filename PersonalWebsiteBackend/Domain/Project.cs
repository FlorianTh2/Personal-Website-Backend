using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PersonalWebsiteBackend.Domain
{
    public class Project : AuditableEntity
    {
        [Key] public Guid Id { get; set; }

        public string Name { get; set; }
        
        public string UserId { get; set; }

        [ForeignKey(nameof(UserId))]
        public ApplicationUser User { get; set; }
    }
}