using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;

namespace PersonalWebsiteBackend.Domain
{
    public class Tag : AuditableEntity
    {
        [Key]
        public string Name { get; set; }
        
        public virtual List<ProjectTag> Projects { get; set; }
        
        public virtual List<DocumentTag> Documents { get; set; }
    }
}