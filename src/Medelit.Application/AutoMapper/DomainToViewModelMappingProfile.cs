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
            CreateMap<Customer, LeadViewModel>()
                .ForMember(dest => dest.AddressStreetName,
                    opts => opts.MapFrom(
                        src => src.HomeStreetName
                    ))
                .ForMember(dest => dest.PreferredPaymentMethodId,
                    opts => opts.MapFrom(
                        src => src.PaymentMethodId
                    ))
                 .ForMember(dest => dest.CityId,
                    opts => opts.MapFrom(
                        src => src.HomeCityId
                    ))
                .ForMember(dest => dest.CountryId,
                    opts => opts.MapFrom(
                        src => src.HomeCountryId
                    ))
                .ForMember(dest => dest.PostalCode,
                    opts => opts.MapFrom(
                        src => src.HomePostCode
                    ))
                    .ReverseMap();
            CreateMap<CustomerServiceRelation, LeadServiceRelationViewModel>();
            CreateMap<Customer, Booking>();
            CreateMap<CustomerServiceRelation, BookingServiceRelation>();

            CreateMap<Professional, ProfessionalRequestViewModel>();
            CreateMap<ProfessionalLanguageRelation, FilterModel>()
                .ForMember(dest => dest.Id,
                    opts => opts.MapFrom(
                        src => src.LanguageId
                    )).ReverseMap();
            CreateMap<Service, ServiceViewModel>();
            CreateMap<LeadServiceRelation, LeadServiceRelationViewModel>();
            CreateMap<CustomerServiceRelation, CustomerServiceRelationViewModel>();

            CreateMap<InvoiceEntity, InvoiceEntityViewModel>((MemberList.Source));
            CreateMap<Invoice, InvoiceViewModel>((MemberList.Source));
            CreateMap<InvoiceServiceRelation, InvoiceServiceRelationViewModel>((MemberList.Source));

            CreateMap<Booking, BookingViewModel>((MemberList.Source));
            CreateMap<BookingServiceRelation, BookingServiceRelationViewModel>((MemberList.Source));
        }
    }
}
