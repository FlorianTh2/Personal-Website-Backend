using System.Collections.Generic;

namespace PersonalWebsiteBackend.Domain
{
    public class GetDocumentsAsyncServiceResponse
    {
        public long TotalDocuments { get; set; }
        
        public List<Document> Documents { get; set; }
    }
}