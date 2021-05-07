using System.Collections.Generic;

namespace PersonalWebsiteBackend.Domain
{
    public class GetProjectsAsyncServiceResponse
    {
        public long TotalProjects { get; set; }
        public List<Project> Projects { get; set; }
    }
}