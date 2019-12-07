using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Medelit.Common;
using Medelit.Domain.Models;

namespace Medelit.Application
{
    public interface IStaticDataService : IDisposable
    {
        dynamic GetProfessionalsForFitler();
        dynamic GePTFeesForFilter();
        dynamic GetPROFeesForFilter();
        IEnumerable<FilterModel> GetFieldsForFilter();
        IEnumerable<FilterModel> GetSubCategoriesForFilter();
        IEnumerable<ContractStatus> GetContractStatusOptions();
        IEnumerable<ApplicationMethod> GetApplicationMethods();
        IEnumerable<ApplicationMean> GetApplicationMeans();
        IEnumerable<DocumentListSent> GetDocumentListSents();
        IEnumerable<CollaborationCode> GetCollaborationCodes();
        IEnumerable<AccountingCode> GetAccountingCodes();
        IEnumerable<BookingStatus> GetBookingStatus();
        IEnumerable<BookingType> GetBookingTypes();
        IEnumerable<BuildingType> GetBuildingTypes();
        IEnumerable<ContactMethod> GetContactMethods();
        IEnumerable<Country> GetCountries();
        IEnumerable<City> GetCities();
        IEnumerable<DiscountNetwork> GetDiscountNewtorks();
        IEnumerable<FilterModel> GetDurations();
        IEnumerable<IERating> GetIERatings();
        IEnumerable<IEType> GetIETypes();
        IEnumerable<InvoiceStatus> GetInvoiceStatuses();
        dynamic GetLanguages();
        
        IEnumerable<LeadCategory> GetLeadCategories();
        IEnumerable<LeadSource> GetLeadSources();
        IEnumerable<LeadStatus> GetLeadStatuses();
        IEnumerable<Title> GetTitles();
        IEnumerable<PaymentMethods> GetPaymentMethods();
        IEnumerable<PaymentStatus> GetPaymentStatuses();
        IEnumerable<Relationship> GetRelationships();
        IEnumerable<FilterModel> GetVats();
        IEnumerable<VisitVenue> GetVisitVenues();
       
    }
}
