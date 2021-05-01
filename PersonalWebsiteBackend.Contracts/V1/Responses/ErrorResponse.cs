using System.Collections.Generic;

namespace PersonalWebsiteBackend.Contracts.V1.Responses
{
    public class ErrorResponse<T>
    {
        public ErrorResponse(){}

        public ErrorResponse(T error)
        {
            Errors.Add(error);
        }
        
        public List<T> Errors { get; set; } = new List<T>();
    }
}