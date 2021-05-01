using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace PersonalWebsiteBackend.Contracts.V1.Requests.Queries
{
    public class GetAllProjectsQuery
    {
        [Required]
        public string UserId { get; set; }
    }
}