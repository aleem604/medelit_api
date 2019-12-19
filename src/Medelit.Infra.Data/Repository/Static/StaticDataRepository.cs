using System.Collections.Generic;
using System.Linq;
using Medelit.Common;
using Medelit.Domain.Interfaces;
using Medelit.Domain.Models;
using Medelit.Infra.Data.Context;
using Medelit.Infra.Data.Repository;
using Microsoft.EntityFrameworkCore;

namespace Equinox.Infra.Data.Repository
{
    public class StaticDataRepository : Repository<StaticData>, IStaticDataRepository
    {
        public StaticDataRepository(MedelitContext context)
            : base(context)
        {}

        public IQueryable<FilterModel> GetCustomersForImportFilter()
        {
            return Db.Customer.Where(x => x.Status == eRecordStatus.Active).Select(x => new FilterModel { Id = x.Id, Value = $"{x.SurName} - {x.DateOfBirth.Value.ToString("yyyy-MM-dd")}" });
        }
        public IQueryable<FilterModel> GetInvoiceEntities()
        {
            return Db.InvoiceEntity.Select(x => new FilterModel { Id = x.Id, Value = x.Name });
        }

        public dynamic GetServicesForFitler()
        {
            return (from s in Db.Service
                    join
                        ptFees in Db.Fee on s.PTFeeId equals ptFees.Id
                    join
                        proFees in Db.Fee on s.PROFeeId equals proFees.Id
                    select new { Id = s.Id, Value = s.Name, ptFeeId = ptFees.Id, ptFeeA1 = ptFees.A1, ptFeeA2 = ptFees.A2, proFeeId = proFees.Id, proFeeA1 = proFees.A1, proFeeA2 = proFees.A2 }).ToList();
        }
        public dynamic GetProfessionalsForFitler(long? serviceId)
        {
            if (serviceId.HasValue)
            {
                return (from p in Db.Professional
                        join sp in Db.ServiceProfessionalRelation on p.Id equals sp.ProfessionalId
                        select new { id = p.Id, Value = p.Name, sid = sp.ServiceId }).ToList();
            }
            else
            {
                return Db.Professional.Select(x => new { x.Id, Value = x.Name }).ToList();
            }
        }

        public dynamic GetPTFeesForFilter()
        {
            return Db.Fee.Where(x => x.FeeTypeId == eFeeType.PTFee).Select(x => new { Id = x.Id, Value = $"{x.FeeCode}-{x.FeeName}", x.A1, x.A2 }).ToList();
        }
        public dynamic GePROFeesForFilter()
        {
            return Db.Fee.Where(x => x.FeeTypeId == eFeeType.PROFee).Select(x => new { Id = x.Id, Value = $"{x.FeeCode}-{x.FeeName}", x.A1, x.A2 }).ToList();
        }

        public IQueryable<FilterModel> GetFieldsForFilter()
        {
            return Db.FieldSubCategory.Select(x => new FilterModel { Id = x.Id, Value = x.Field });
        }

        public IQueryable<FilterModel> GetSubCategoriesForFilter()
        {
            return Db.FieldSubCategory.Select(x => new FilterModel { Id = x.Id, Value = x.SubCategory }).Where(x => x.Value != null);
        }

        public IQueryable<ContractStatus> GetContractStatus()
        {
            //return Db.ContactStatus;
            return Db.StaticData.Select((s) => new ContractStatus { Id = s.Id, Value = s.ContractStatus }).Where(x => x.Value != null);
        }

        public IQueryable<ApplicationMethod> GetApplicationMethods()
        {
            //return Db.ApplicationMethod;
            return Db.StaticData.Select((s) => new ApplicationMethod { Id = s.Id, Value = s.ApplicationMethods }).Where(x => x.Value != null);
        }

        public IQueryable<ApplicationMean> GetApplicationMeans()
        {
            //return Db.ApplicationMean;
            return Db.StaticData.Select((s) => new ApplicationMean { Id = s.Id, Value = s.ApplicationMeans }).Where(x=> x.Value != null);
        }

        public IQueryable<DocumentListSent> GetDocumentListSents()
        {
            //return Db.DocumentListSent;
            return Db.StaticData.Select((s) => new DocumentListSent { Id = s.Id, Value = s.DocumentListSentOptions }).Where(x => x.Value != null);
        }

        public IQueryable<AccountingCode> GetAccountingCodes()
        {
            //return Db.AccountingCode;
            return Db.StaticData.Select((s) => new AccountingCode { Id = s.Id, Value = s.AccountingCodes }).Where(x => x.Value != null);
        }

        public IQueryable<CollaborationCode> GetCollaborationCodes()
        {
            //return Db.CollaborationCode;
            return Db.StaticData.Select((s) => new CollaborationCode { Id = s.Id, Value = s.CollaborationCodes }).Where(x => x.Value != null);
        }

        public IQueryable<BookingStatus> GetBookingStatus()
        {
            //return Db.BookingStatus;
            return Db.StaticData.Select((s) => new BookingStatus { Id = s.Id, Value = s.BookingStatus }).Where(x => x.Value != null);
        }

        public IQueryable<BookingType> GetBookingTypes()
        {
            //return Db.BookingType;
            return Db.StaticData.Select((s) => new BookingType { Id = s.Id, Value = s.BookingTypes }).Where(x => x.Value != null);
        }

        public IQueryable<BuildingType> GetBuildingTypes()
        {
            //return Db.BuildingType;
            return Db.StaticData.Select((s) => new BuildingType { Id = s.Id, Value = s.BuildingTypes }).Where(x => x.Value != null);
        }

        public IQueryable<ContactMethod> GetContactMethods()
        {
            //return Db.ContactMethods;
            return Db.StaticData.Select((s) => new ContactMethod { Id = s.Id, Value = s.ContactMethods }).Where(x => x.Value != null);
        }

        public IQueryable<Country> GetCountries()
        {
            return Db.Countries;
        }

        public IQueryable<City> GetCities()
        {
            return Db.City;
        }

        public IQueryable<DiscountNetwork> GetDiscountNewtorks()
        {
            //return Db.DiscountNetworks;
            return Db.StaticData.Select((s) => new DiscountNetwork { Id = s.Id, Value = s.DiscountNetworks }).Where(x => x.Value != null);
        }

        public IQueryable<Duration> GetDurations()
        {
            return Db.Durations;
            //return Db.StaticData.Select((s) => new Duration { Id = s.Id, Value = $"{s.Durations} {s.DurationUnits}" }).Where(x => x.Value != null);
        }

        public IQueryable<IERating> GetIERatings()
        {
            //return Db.IERatings;
            return Db.StaticData.Select((s) => new IERating { Id = s.Id, Value = s.IERatings }).Where(x => x.Value != null);
        }

        public IQueryable<IEType> GetIETypes()
        {
            //return Db.IETypes;
            return Db.StaticData.Select((s) => new IEType { Id = s.Id, Value = s.IETypes }).Where(x => x.Value != null);
        }

        public IQueryable<InvoiceStatus> GetInvoiceStatuses()
        {
            //return Db.InvoiceStatus;
            return Db.StaticData.Select((s) => new InvoiceStatus { Id = s.Id, Value = s.InvoiceStatus }).Where(x => x.Value != null);
        }

        public IQueryable<FilterModel> GetLanguages()
        {
            return Db.Languages.Select(x => new FilterModel { Id = x.Id, Value = x.Name });
        }

        public IQueryable<LeadCategory> GetLeadCategories()
        {
            return Db.LeadCategories;
        }

        public IQueryable<LeadSource> GetLeadSources()
        {
            //return Db.LeadSources;
            return Db.StaticData.Select((s) => new LeadSource { Id = s.Id, Value = s.LeadSources }).Where(x => x.Value != null);
        }

        public IQueryable<LeadStatus> GetLeadStatuses()
        {
            //return Db.LeadStatus;
            return Db.StaticData.Select((s) => new LeadStatus { Id = s.Id, Value = s.LeadStatus }).Where(x => x.Value != null);
        }

        public IQueryable<Title> GetTitles()
        {
            //return Db.Titles;
            return Db.StaticData.Select((s) => new Title { Id = s.Id, Value = s.Titles }).Where(x => x.Value != null);
        }

        public IQueryable<PaymentMethods> GetPaymentMethods()
        {
            //return Db.PaymentMethods;
            return Db.StaticData.Select((s) => new PaymentMethods { Id = s.Id, Value = s.PaymentMethods }).Where(x => x.Value != null);
        }

        public IQueryable<PaymentStatus> GetPaymentStatuses()
        {
            //return Db.PaymentStatus;
            return Db.StaticData.Select((s) => new PaymentStatus { Id = s.Id, Value = s.PaymentStatus }).Where(x => x.Value != null);
        }

        public IQueryable<Relationship> GetRelationships()
        {
            //return Db.Relationships;
            return Db.StaticData.Select((s) => new Relationship { Id = s.Id, Value = s.Relationships }).Where(x => x.Value != null);
        }

        public IQueryable<Vat> GetVats()
        {
            return Db.Vats;
            //return Db.StaticData.Select((s) => new Vat { Id = s.Id, Value = s.Vats }).Where(x => x.Value != null);
        }

        public IQueryable<VisitVenue> GetVisitVenues()
        {
            //return Db.VisitVenues;
            return Db.StaticData.Select((s) => new VisitVenue { Id = s.Id, Value = s.VisitVenues }).Where(x => x.Value != null);
        }

        public IQueryable<ReportDeliveryOptions> GetReportDeliveryOptions()
        {
            //return Db.ReportDeliveryOptions;
            return Db.StaticData.Select((s) => new ReportDeliveryOptions { Id = s.Id, Value = s.ReportDeliveryOptions }).Where(x => x.Value != null);
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

    }
}
