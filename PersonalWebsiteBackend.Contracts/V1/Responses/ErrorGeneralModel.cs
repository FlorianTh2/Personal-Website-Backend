namespace PersonalWebsiteBackend.Contracts.V1.Responses
{
    public class ErrorGeneralModel : ErrorModel
    {
        public string Type { get; set; }
        public string StackTrace { get; set; }
        public string Message { get; set; }
    }
}