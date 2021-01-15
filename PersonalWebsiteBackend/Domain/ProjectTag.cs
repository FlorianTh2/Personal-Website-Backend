using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace PersonalWebsiteBackend.Domain
{
    public class ProjectTag
    {
        // property
        public string TagName { get; set; }

        // navigational-property
        [ForeignKey(nameof(TagName))]
        public virtual Tag Tag { get; set; }
        
        // property
        public Guid ProjectId { get; set; }

        // navigational-property
        public virtual Project Project { get; set; }

    }
}