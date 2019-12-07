using AutoMapper;
using Medelit.Common;
using Medelit.Domain.Models;

namespace Medelit.Application.AutoMapper
{
    public class DomainToViewModelMappingProfile : Profile
    {
        public DomainToViewModelMappingProfile()
        {
            CreateMap<Lead, LeadViewModel>();
            CreateMap<Professional, ProfessionalRequestViewModel>();
            CreateMap<ProfessionalLanguageRelation, FilterModel>()
                .ForMember(dest => dest.Id,
                    opts => opts.MapFrom(
                        src => src.LanguageId
                    )).ReverseMap();
            CreateMap<Service, ServiceViewModel>();


        }
    }
}
