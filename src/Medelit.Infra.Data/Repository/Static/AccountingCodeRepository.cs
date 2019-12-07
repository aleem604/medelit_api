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
    public class AccountingCodeRepository : Repository<AccountingCode>, IAccountingCodeRepository
    {
        public AccountingCodeRepository(MedelitContext context)
            : base(context)
        {

        }

        public dynamic GetProfessionalsForFitler()
        {
            return Db.Professional.Select(x => new { x.Id, Value = x.Name }).ToList();
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
            return Db.FieldSubCategory.Select(x => new FilterModel { Id = x.Id, Value = x.SubCategory });
        }

        public IQueryable<ContractStatus> GetContractStatus()
        {
            return Db.ContactStatus;
        }

        public IQueryable<ApplicationMethod> GetApplicationMethods()
        {
            return Db.ApplicationMethod;
        }

        public IQueryable<ApplicationMean> GetApplicationMeans()
        {
            return Db.ApplicationMean;
        }

        public IQueryable<DocumentListSent> GetDocumentListSents()
        {
            return Db.DocumentListSent;
        }

        public IQueryable<AccountingCode> GetAccountingCodes()
        {
            return Db.AccountingCode;
        }

        public IQueryable<CollaborationCode> GetCollaborationCodes()
        {
            return Db.CollaborationCode;
        }

        public IQueryable<BookingStatus> GetBookingStatus()
        {
            return Db.BookingStatus;
        }

        public IQueryable<BookingType> GetBookingTypes()
        {
            return Db.BookingType;
        }

        public IQueryable<BuildingType> GetBuildingTypes()
        {
            return Db.BuildingType;
        }

        public IQueryable<ContactMethod> GetContactMethods()
        {
            return Db.ContactMethods;
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
            return Db.DiscountNetworks;
        }

        public IQueryable<Duration> GetDurations()
        {
            return Db.Durations;
        }

        public IQueryable<IERating> GetIERatings()
        {
            return Db.IERatings;
        }

        public IQueryable<IEType> GetIETypes()
        {
            return Db.IETypes;
        }

        public IQueryable<InvoiceStatus> GetInvoiceStatuses()
        {
            return Db.InvoiceStatus;
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
            return Db.LeadSources;
        }

        public IQueryable<LeadStatus> GetLeadStatuses()
        {
            return Db.LeadStatus;
        }

        public IQueryable<Title> GetTitles()
        {
            return Db.Titles;
        }

        public IQueryable<PaymentMethods> GetPaymentMethods()
        {
            return Db.PaymentMethods;
        }

        public IQueryable<PaymentStatus> GetPaymentStatuses()
        {
            return Db.PaymentStatus;
        }

        public IQueryable<Relationship> GetRelationships()
        {
            return Db.Relationships;
        }

        public IQueryable<Vat> GetVats()
        {
            return Db.Vats;
        }

        public IQueryable<VisitVenue> GetVisitVenues()
        {
            return Db.VisitVenues;
        }
    }
}
