namespace PersonalWebsiteBackend.Contracts.V1.Responses
{
    public class ErrorFieldValidationModel : ErrorModel
    {
        public string FieldName { get; set; }

        public string Message { get; set; }
    }
}