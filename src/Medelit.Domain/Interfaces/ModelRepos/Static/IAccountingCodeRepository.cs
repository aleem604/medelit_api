using Medelit.Common;
using Medelit.Domain.Models;
using System.Collections.Generic;
using System.Linq;

namespace Medelit.Domain.Interfaces
{
    public interface IAccountingCodeRepository : IRepository<AccountingCode>
    {
        dynamic GetProfessionalsForFitler();
        dynamic GetPTFeesForFilter();
        dynamic GePROFeesForFilter();
        IQueryable<FilterModel> GetFieldsForFilter();
        IQueryable<FilterModel> GetSubCategoriesForFilter();
        IQueryable<ContractStatus> GetContractStatus();
        IQueryable<ApplicationMethod> GetApplicationMethods();
        IQueryable<ApplicationMean> GetApplicationMeans();
        IQueryable<DocumentListSent> GetDocumentListSents();
        IQueryable<AccountingCode> GetAccountingCodes();
        IQueryable<CollaborationCode> GetCollaborationCodes();
        IQueryable<BookingStatus> GetBookingStatus();
        IQueryable<BookingType> GetBookingTypes();
        IQueryable<BuildingType> GetBuildingTypes();
        IQueryable<ContactMethod> GetContactMethods();
        IQueryable<Country> GetCountries();
        IQueryable<City> GetCities();
        IQueryable<DiscountNetwork> GetDiscountNewtorks();
        IQueryable<Duration> GetDurations();
        IQueryable<IERating> GetIERatings();
        IQueryable<IEType> GetIETypes();
        IQueryable<InvoiceStatus> GetInvoiceStatuses();
        IQueryable<FilterModel> GetLanguages();

        IQueryable<LeadCategory> GetLeadCategories();
        IQueryable<LeadSource> GetLeadSources();
        IQueryable<LeadStatus> GetLeadStatuses();
        IQueryable<Title> GetTitles();
        IQueryable<PaymentMethods> GetPaymentMethods();
        IQueryable<PaymentStatus> GetPaymentStatuses();
        IQueryable<Relationship> GetRelationships();
        IQueryable<Vat> GetVats();
        IQueryable<VisitVenue> GetVisitVenues();
    }
}