using AutoMapper;
using Medelit.Common;
using Medelit.Domain;
using Medelit.Domain.Commands;
using Medelit.Domain.Events;
using Medelit.Domain.Models;

namespace Medelit.Application.AutoMapper
{
    public class ViewModelToDomainMappingProfile : Profile
    {
        public ViewModelToDomainMappingProfile()
        {
            
            //CreateMap<Source, Destination>()
            //    .ForMember(d => d.Text, o => o.MapFrom(s => s.Name))
            //    .ForMember(d => d.Value, o => o.MapFrom(s => s.Id))
            //    .ForAllOtherMembers(opts => opts.Ignore());

            CreateMap<FeeViewModel, Fee>((MemberList.Source));
            CreateMap<ProfessionalViewModel, Professional>((MemberList.Destination));
            CreateMap<FilterModel, ProfessionalLanguages>()
               .ForMember(dest => dest.LanguageId,
                   opts => opts.MapFrom(
                       src => src.Id
                   )).ReverseMap();
            // Services
            CreateMap<ServiceViewModel, Service>((MemberList.Source));
            CreateMap<AddUpdateFeeToServiceViewModel, AddUpdateFeeToService>((MemberList.Source));

            // Leads
            CreateMap<LeadViewModel, Lead>((MemberList.Source));
            CreateMap<LeadServiceRelationViewModel, LeadServiceRelation>();
            CreateMap<Lead, Customer>((MemberList.Source));
            CreateMap<LeadServiceRelation, CustomerServiceRelation>((MemberList.Source));


            CreateMap<CustomerViewModel, Customer>((MemberList.Source));
            CreateMap<CustomerServiceRelationViewModel, CustomerServiceRelation>((MemberList.Source));

            CreateMap<InvoiceEntityViewModel, InvoiceEntity>((MemberList.Source));

            CreateMap<BookingViewModel, Booking>((MemberList.Source)).ForMember(dest => dest.IsAllDayVisit,
                   opts => opts.MapFrom(
                       src => src.IsAllDayVisit.HasValue && src.IsAllDayVisit.Value == true ? 1 : 0
                   )).ReverseMap();
           

        }
    }
}
