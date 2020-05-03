using AutoMapper;
using Medelit.Common;
using Medelit.Domain.Models;
using Medelit.Infra.CrossCutting.Identity.Models;

namespace Medelit.Application.AutoMapper
{
    public class DomainToViewModelMappingProfile : Profile
    {
        public DomainToViewModelMappingProfile()
        {
            CreateMap<MedelitUser, UserViewModel>();

            CreateMap<PtFee, FeeViewModel>();
            CreateMap<Lead, LeadViewModel>();
            CreateMap<Customer, CustomerViewModel>((MemberList.Source));
            CreateMap<Customer, LeadViewModel>()
                .ForMember(dest => dest.AddressStreetName,
                    opts => opts.MapFrom(
                        src => src.HomeStreetName
                    ))
                .ForMember(dest => dest.PreferredPaymentMethodId,
                    opts => opts.MapFrom(
                        src => src.PaymentMethodId
                    ))
                 .ForMember(dest => dest.City,
                    opts => opts.MapFrom(
                        src => src.HomeCity
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
            CreateMap<CustomerServices, LeadServiceRelationViewModel>();
            CreateMap<Customer, Booking>();

            CreateMap<VFees, FeeViewModel>();

            CreateMap<Professional, ProfessionalViewModel>();
            CreateMap<ProfessionalLanguages, FilterModel>()
                .ForMember(dest => dest.Id,
                    opts => opts.MapFrom(
                        src => src.LanguageId
                    ))
                .ForMember(dest => dest.Value,
                    opts => opts.Ignore())
                 .ForMember(dest => dest.DecValue,
                    opts => opts.Ignore())
                 .ForMember(dest => dest.Email,
                    opts => opts.Ignore())
                .ReverseMap();


            CreateMap<ProfessionalFields, FilterModel>()
               .ForMember(dest => dest.Id,
                   opts => opts.MapFrom(
                       src => src.FieldId
                   )).ReverseMap();
            CreateMap<ProfessionalSubCategories, FilterModel>()
               .ForMember(dest => dest.Id,
                   opts => opts.MapFrom(
                       src => src.SubCategoryId
                   )).ReverseMap();
            CreateMap<Service, ServiceViewModel>();
            //CreateMap<ServiceProfessionalRelation, ServiceProfessionalRelationVeiwModel>();
            CreateMap<FieldSubCategory, FieldSubCategoryViewModel>();

            CreateMap<LeadServices, LeadServiceRelationViewModel>();
            CreateMap<CustomerServices, CustomerServiceRelationViewModel>();

            CreateMap<InvoiceEntity, InvoiceEntityViewModel>((MemberList.Source));
            CreateMap<Invoice, InvoiceViewModel>((MemberList.Source));
            CreateMap<InvoiceBookings, InvoiceBookingsViewModel>((MemberList.Source));
            

            CreateMap<Booking, BookingViewModel>((MemberList.Source)).ForMember(dest => dest.IsAllDayVisit,
                   opts => opts.MapFrom(
                       src => src.IsAllDayVisit.Value == 0 ? false : true
                   )).ReverseMap();
           
        }
    }
}
