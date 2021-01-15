using AutoMapper;
using PersonalWebsiteBackend.Contracts.V1.Requests.Queries;
using PersonalWebsiteBackend.Domain;

namespace PersonalWebsiteBackend.Mapping
{
    public class RequestToDomainProfile : Profile
    {
        public RequestToDomainProfile()
        {
            CreateMap<PaginationQuery, PaginationFilter>();
            CreateMap<GetAllProjectsQuery, GetAllProjectsFilter>();
        }
    }
}