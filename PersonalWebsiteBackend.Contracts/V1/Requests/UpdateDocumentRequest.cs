namespace PersonalWebsiteBackend.Contracts.V1.Requests
{
    public class UpdateDocumentRequest
    {
        public string Name { get; set; }

        public string Description { get; set; }

        public string Topic { get; set; }

        public string Link { get; set; }
    }
}