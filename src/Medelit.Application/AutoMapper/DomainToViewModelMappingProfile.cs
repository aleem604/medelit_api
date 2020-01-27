﻿using AutoMapper;
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

            CreateMap<VFees, FeeViewModel>();

            CreateMap<Professional, ProfessionalViewModel>();
            CreateMap<ProfessionalLanguages, FilterModel>()
                .ForMember(dest => dest.Id,
                    opts => opts.MapFrom(
                        src => src.LanguageId
                    )).ReverseMap();
            CreateMap<Service, ServiceViewModel>();
            //CreateMap<ServiceProfessionalRelation, ServiceProfessionalRelationVeiwModel>();
            CreateMap<FieldSubCategory, FieldSubCategoryViewModel>();

            CreateMap<LeadServiceRelation, LeadServiceRelationViewModel>();
            CreateMap<CustomerServiceRelation, CustomerServiceRelationViewModel>();

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
