using System.Collections.Generic;
using System.Linq;
using Medelit.Common;
using Medelit.Domain.Core.Bus;
using Medelit.Domain.Interfaces;
using Medelit.Domain.Models;
using Medelit.Infra.Data.Context;
using Medelit.Infra.Data.Repository;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace Medelit.Infra.Data.Repository
{

    public class StaticDataRepository : Repository<StaticData>, IStaticDataRepository
    {
        public StaticDataRepository(MedelitContext context, IHttpContextAccessor contextAccessor, IMediatorHandler bus)
            : base(context, contextAccessor, bus)
        { }

        public IQueryable<FilterModel> GetCustomersForImportFilter()
        {
            return Db.Customer.Where(x => x.Status == eRecordStatus.Active).Select(x => new FilterModel { Id = x.Id, Value = $"{x.Name} {x.SurName} - {x.DateOfBirth.CustomDateString("dd/MM/yyyy")}" });
        }

        public IQueryable<FilterModel> GetCustomersForFilter()
        {
            return Db.Customer.Where(x => x.Status == eRecordStatus.Active).Select(x => new FilterModel { Id = x.Id, Value = $"{x.SurName} {x.Name}" });
        }

        public IQueryable<FilterModel> GetInvoicesForFilter()
        {
            return Db.Invoice.Where(x => x.Status == eRecordStatus.Active).Select(x => new FilterModel { Id = x.Id, Value = x.Subject });
        }

        public IQueryable<FilterModel> GetInvoiceEntities()
        {
            return Db.InvoiceEntity.Select(x => new FilterModel { Id = x.Id, Value = x.Name });
        }

        public dynamic GetServicesForFitler()
        {
            var vats = Db.StaticData.Select((s) => new FilterModel { Id = s.Id, Value = $"{s.Vats} {s.VatUnit}", DecValue = s.Vats }).Where(x => x.Value != null);

            return (from v in Db.ServiceProfessionalFees
                    select new
                    {
                        Id = v.ServiceId,
                        Value = v.Service.Name,
                        timeService = v.Service.TimedServiceId,
                        vat = v.Service.VatId.HasValue ? vats.FirstOrDefault(x => x.Id == v.Service.VatId.Value).DecValue : null
                    }).DistinctBy(x => x.Id).ToList();
        }

        public dynamic GetProfessionalsWithFeesForFitler(long? serviceId)
        {
            var ptFees = Db.ServiceProfessionalFees.Include(x => x.PtFee).ToList();
            var proFees = Db.ServiceProfessionalFees.Include(x => x.ProFee).ToList();

            return (from v in Db.ServiceProfessionalFees
                    where 
                    /*v.ServiceId == serviceId &&*/ 
                    v.PtFeeId.HasValue && v.ProFeeId.HasValue
                    select new
                    {
                        spfId= v.Id,
                        id = v.ProfessionalId,
                        value = v.Professional.Name,
                        sid = v.ServiceId,
                        ptFees = new { v.ServiceId, v.ProfessionalId, v.PtFee.FeeName, id = v.PtFeeId, v.PtFee.A1, v.PtFee.A2 },
                        proFees = new { v.ServiceId, v.ProfessionalId, v.ProFee.FeeName, id = v.ProFeeId, v.ProFee.A1, v.ProFee.A2 },
                    }).ToList();
        }

        private IEnumerable<object> GetPtFeeList(IEnumerable<ServiceProfessionalFees> ptFees, long? serviceId, long? professionalId)
        {
            return ptFees.Where(x => x.ProfessionalId == professionalId && x.ServiceId == serviceId && x.PtFeeId.HasValue)
                .Select(x => new
                {
                    Id = x.PtFeeId,
                    value = x.PtFee.FeeName,
                    A1 = x.PtFee.A1,
                    A2 = x.PtFee.A2
                }).DistinctBy(x => x.Id).ToList();
        }

        private IEnumerable<object> GetProFeeList(IEnumerable<ServiceProfessionalFees> proFees, long? serviceId, long? professionalId)
        {
            return proFees.Where(x => x.ProfessionalId == professionalId && x.ServiceId == serviceId && x.ProFeeId.HasValue)
                .Select(x => new
                {
                    Id = x.ProFeeId,
                    Value = x.ProFee.FeeName,
                    A1 = x.ProFee.A1,
                    A2 = x.ProFee.A2
                }).DistinctBy(x => x.Id).ToList();
        }

        public dynamic GetProfessionalsForFitler(long? serviceId)
        {
            if (serviceId.HasValue)
            {
                var result = (from p in Db.Professional
                              join sp in Db.ServiceProfessionalFees on p.Id equals sp.ProfessionalId
                              select new { id = p.Id, Value = p.Name, sid = sp.ServiceId }).ToList();
                if (result is null)
                    return Db.Professional.Select(x => new { x.Id, Value = x.Name }).ToList();
                else
                    return result;
            }

            return Db.Professional.Select(x => new { x.Id, Value = x.Name }).ToList();
        }

        public dynamic GetPTFeesForFilter()
        {
            return Db.PtFee.Where(x => x.FeeTypeId == eFeeType.PTFee).Select(x => new { Id = x.Id, Value = $"{x.FeeCode}-{x.FeeName}", x.A1, x.A2 }).ToList();
        }
        public dynamic GePROFeesForFilter()
        {
            return Db.PtFee.Where(x => x.FeeTypeId == eFeeType.PROFee).Select(x => new { Id = x.Id, Value = $"{x.FeeCode}-{x.FeeName}", x.A1, x.A2 }).ToList();
        }

        public IQueryable<FilterModel> GetFieldsForFilter()
        {
            return Db.FieldSubCategory.Where(x => !string.IsNullOrEmpty(x.Field)).Select(x => new FilterModel { Id = x.Id, Value = x.Field });
        }

        public IQueryable<FilterModel> GetSubCategoriesForFilter(IEnumerable<FilterModel> fields)
        {
            var fieldNames = Db.FieldSubCategory.Where(x => fields.Select(s => s.Id).Contains(x.Id)).Select(x => x.Field).Distinct().ToList();

            return Db.FieldSubCategory.Where(x => fieldNames.Contains(x.Field)).Select(x => new FilterModel { Id = x.Id, Value = x.SubCategory }).Where(x => x.Value != null);
        }

        public IQueryable<FilterModel> GetContractStatus()
        {
            //return Db.ContactStatus;
            return Db.StaticData.Select((s) => new FilterModel { Id = s.Id, Value = s.ContractStatus }).Where(x => x.Value != null);
        }

        public IQueryable<FilterModel> GetApplicationMethods()
        {
            //return Db.ApplicationMethod;
            return Db.StaticData.Select((s) => new FilterModel { Id = s.Id, Value = s.ApplicationMethods }).Where(x => x.Value != null);
        }

        public IQueryable<FilterModel> GetApplicationMeans()
        {
            //return Db.ApplicationMean;
            return Db.StaticData.Select((s) => new FilterModel { Id = s.Id, Value = s.ApplicationMeans }).Where(x => x.Value != null);
        }

        public IQueryable<FilterModel> GetDocumentListSents()
        {
            //return Db.DocumentListSent;
            return Db.StaticData.Select((s) => new FilterModel { Id = s.Id, Value = s.DocumentListSentOptions }).Where(x => x.Value != null);
        }

        public IQueryable<FilterModel> GetAccountingCodes()
        {
            //return Db.AccountingCode;
            return Db.StaticData.Select((s) => new FilterModel { Id = s.Id, Value = s.AccountingCodes }).Where(x => x.Value != null);
        }

        public IQueryable<FilterModel> GetCollaborationCodes()
        {
            //return Db.CollaborationCode;
            return Db.StaticData.Select((s) => new FilterModel { Id = s.Id, Value = s.CollaborationCodes }).Where(x => x.Value != null);
        }

        public IQueryable<FilterModel> GetBookingStatus()
        {
            //return Db.BookingStatus;
            return Db.StaticData.Select((s) => new FilterModel { Id = s.Id, Value = s.BookingStatus }).Where(x => x.Value != null);
        }

        public IQueryable<FilterModel> GetBookingTypes()
        {
            //return Db.BookingType;
            return Db.StaticData.Select((s) => new FilterModel { Id = s.Id, Value = s.BookingTypes }).Where(x => x.Value != null);
        }

        public IQueryable<FilterModel> GetBuildingTypes()
        {
            //return Db.BuildingType;
            return Db.StaticData.Select((s) => new FilterModel { Id = s.Id, Value = s.BuildingTypes }).Where(x => x.Value != null);
        }

        public IQueryable<FilterModel> GetContactMethods()
        {
            //return Db.ContactMethods;
            return Db.StaticData.Select((s) => new FilterModel { Id = s.Id, Value = s.ContactMethods }).Where(x => x.Value != null);
        }

        public IQueryable<Country> GetCountries()
        {
            return Db.Countries;
        }

        public IQueryable<City> GetCities()
        {
            return Db.City;
        }

        public IQueryable<FilterModel> GetDiscountNewtorks()
        {
            //return Db.DiscountNetworks;
            return Db.StaticData.Select((s) => new FilterModel { Id = s.Id, Value = s.DiscountNetworks }).Where(x => x.Value != null);
        }

        public IQueryable<FilterModel> GetDurations()
        {
            return Db.StaticData.Select((s) => new FilterModel { Id = s.Id, Value = $"{s.Durations} {s.DurationUnits}" }).Where(x => x.Value != null);
        }

        public IQueryable<FilterModel> GetIERatings()
        {
            //return Db.IERatings;
            return Db.StaticData.Select((s) => new FilterModel { Id = s.Id, Value = s.IERatings }).Where(x => x.Value != null);
        }

        public IQueryable<FilterModel> GetIETypes()
        {
            //return Db.IETypes;
            return Db.StaticData.Select((s) => new FilterModel { Id = s.Id, Value = s.IETypes }).Where(x => x.Value != null);
        }

        public IQueryable<FilterModel> GetInvoiceStatuses()
        {
            //return Db.InvoiceStatus;
            return Db.StaticData.Select((s) => new FilterModel { Id = s.Id, Value = s.InvoiceStatus }).Where(x => x.Value != null);
        }

        public IQueryable<FilterModel> GetLanguages()
        {
            return Db.Languages.Select(x => new FilterModel { Id = x.Id, Value = x.Name });
        }

        public IQueryable<FilterModel> GetLeadCategories()
        {
            return Db.StaticData.Select((s) => new FilterModel { Id = s.Id, Value = s.LeadCategories }).Where(x => x.Value != null);
        }

        public IQueryable<FilterModel> GetLeadSources()
        {
            //return Db.LeadSources;
            return Db.StaticData.Select((s) => new FilterModel { Id = s.Id, Value = s.LeadSources }).Where(x => x.Value != null);
        }

        public IQueryable<FilterModel> GetLeadStatuses()
        {
            //return Db.LeadStatus;
            return Db.StaticData.Select((s) => new FilterModel { Id = s.Id, Value = s.LeadStatus }).Where(x => x.Value != null);
        }

        public IQueryable<FilterModel> GetTitles()
        {
            //return Db.Titles;
            return Db.StaticData.Select((s) => new FilterModel { Id = s.Id, Value = s.Titles }).Where(x => x.Value != null);
        }

        public IQueryable<FilterModel> GetPaymentMethods()
        {
            //return Db.PaymentMethods;
            return Db.StaticData.Select((s) => new FilterModel { Id = s.Id, Value = s.PaymentMethods }).Where(x => x.Value != null);
        }

        public IQueryable<FilterModel> GetPaymentStatuses()
        {
            //return Db.PaymentStatus;
            return Db.StaticData.Select((s) => new FilterModel { Id = s.Id, Value = s.PaymentStatus }).Where(x => x.Value != null);
        }

        public IQueryable<FilterModel> GetRelationships()
        {
            //return Db.Relationships;
            return Db.StaticData.Select((s) => new FilterModel { Id = s.Id, Value = s.Relationships }).Where(x => x.Value != null);
        }

        public IQueryable<FilterModel> GetVats()
        {
            return Db.StaticData.Select((s) => new FilterModel { Id = s.Id, Value = $"{s.Vats} {s.VatUnit}", DecValue = s.Vats.Value }).Where(x => x.Value != null);
        }

        public IQueryable<FilterModel> GetVisitVenues()
        {
            //return Db.VisitVenues;
            return Db.StaticData.Select((s) => new FilterModel { Id = s.Id, Value = s.VisitVenues }).Where(x => x.Value != null);
        }
        public IQueryable<FilterModel>  GetProTaxCodes()
        {
            //return Db.VisitVenues;
            return Db.StaticData.Select((s) => new FilterModel { Id = s.Id, Value = s.ProTaxCodes }).Where(x => x.Value != null);
        }

        public IQueryable<FilterModel> GetReportDeliveryOptions()
        {
            //return Db.ReportDeliveryOptions;
            return Db.StaticData.Select((s) => new FilterModel { Id = s.Id, Value = s.ReportDeliveryOptions }).Where(x => x.Value != null);
        }

        public IQueryable<FilterModel> GetAddedToAccountOptions()
        {
            return Db.StaticData.Where(x => x.AddToAccountOptions != null).Select(x => new FilterModel { Id = x.Id, Value = x.AddToAccountOptions });
        }

        public IQueryable<StaticData> GetStaticData()
        {
            return Db.StaticData;
        }

        public IEnumerable<FilterModel> GetLabs()
        {
            return Db.Lab.Select(x => new FilterModel { Id = x.Id, Value = x.Name });
        }

        public dynamic GetAccountInfo()
        {
            return Db.CompanyAccountInfo.FirstOrDefault();
        }

    }
}

