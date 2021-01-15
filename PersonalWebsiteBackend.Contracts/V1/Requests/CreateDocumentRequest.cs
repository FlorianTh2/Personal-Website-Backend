using System.Collections.Generic;

namespace PersonalWebsiteBackend.Contracts.V1.Requests
{
    public class CreateDocumentRequest
    {
        public string Name { get; set; }

        public string Description { get; set; }

        public string Topic { get; set; }

        public string Link { get; set; }
        
        public IEnumerable<string> Tags { get; set; }
    }
}