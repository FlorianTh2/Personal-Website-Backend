using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PersonalWebsiteBackend.Contracts.V1.Responses
{
    public class DocumentResponse
    { 
        public Guid Id { get; set; }
        
        public string DocumentId { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public string OwnersEmail { get; set; }

        public string ThumbnailLink { get; set; }

        public string WebcontentLink { get; set; }

        public string WebviewLink { get; set; }

        public string FileExtension { get; set; }

        public string FullFileExtension { get; set; }

        public string Kind { get; set; }

        public string Md5Checksum { get; set; }

        public bool Shared { get; set; }

        public long Size { get; set; }

        public long Version { get; set; }

        public string DocumentCreatedTime { get; set; }
        
        public string UserId { get; set; }
        
        public DateTime CreatedOn { get; set; }

        public string CreatorId { get; set; }

        public DateTime UpdatedOn { get; set; }

        public string UpdaterId { get; set; }
    }
}