using System.Linq;
using AutoMapper;
using PersonalWebsiteBackend.Contracts.V1.Responses;
using PersonalWebsiteBackend.Domain;

namespace PersonalWebsiteBackend.Mapping
{
    public class DomainToResponseProfile : Profile
    {
        public DomainToResponseProfile()
        {
            CreateMap<Project, ProjectResponse>();

            CreateMap<Document, DocumentResponse>();
        }
    }
}