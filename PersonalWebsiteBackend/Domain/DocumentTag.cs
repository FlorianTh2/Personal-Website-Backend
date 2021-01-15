using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace PersonalWebsiteBackend.Domain
{
    public class DocumentTag
    {
        // property
        public string TagName { get; set; }

        [ForeignKey(nameof(TagName))]
        public virtual Tag Tag { get; set; }
        
        public Guid DocumentId { get; set; }

        public virtual Document Document { get; set; }
    }
}