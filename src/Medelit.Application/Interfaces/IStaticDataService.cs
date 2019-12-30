using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Medelit.Common;
using Medelit.Domain.Models;

namespace Medelit.Application
{
    public interface IStaticDataService : IDisposable
    {
        IEnumerable<FilterModel> GetCustomersForImportFilter();
        IEnumerable<FilterModel> GetInvoicesForFilter();
        IEnumerable<FilterModel> GetInvoiceEntities();
        dynamic GetServicesForFitler();
        dynamic GetProfessionalsForFitler(long? serviceId);
        dynamic GePTFeesForFilter();
        dynamic GetPROFeesForFilter();
        IEnumerable<FilterModel> GetFieldsForFilter();
        IEnumerable<FilterModel> GetSubCategoriesForFilter();
        IEnumerable<FilterModel> GetContractStatusOptions();
        IEnumerable<FilterModel> GetApplicationMethods();
        IEnumerable<FilterModel> GetApplicationMeans();
        IEnumerable<FilterModel> GetDocumentListSents();
        IEnumerable<FilterModel> GetCollaborationCodes();
        IEnumerable<FilterModel> GetAccountingCodes();
        IEnumerable<FilterModel> GetBookingStatus();
        IEnumerable<FilterModel> GetBookingTypes();
        IEnumerable<FilterModel> GetBuildingTypes();
        IEnumerable<FilterModel> GetContactMethods();
        IEnumerable<Country> GetCountries();
        IEnumerable<City> GetCities();
        
        IEnumerable<FilterModel> GetDiscountNewtorks();
        IEnumerable<FilterModel> GetDurations();
        IEnumerable<FilterModel> GetIERatings();
        IEnumerable<FilterModel> GetIETypes();
        IEnumerable<FilterModel> GetInvoiceStatuses();
        dynamic GetLanguages();
        
        IEnumerable<FilterModel> GetLeadCategories();
        IEnumerable<FilterModel> GetLeadSources();
        IEnumerable<FilterModel> GetLeadStatuses();
        IEnumerable<FilterModel> GetTitles();
        IEnumerable<FilterModel> GetPaymentMethods();
        IEnumerable<FilterModel> GetPaymentStatuses();
        IEnumerable<FilterModel> GetRelationships();
        IEnumerable<FilterModel> GetVats();
        IEnumerable<FilterModel> GetVisitVenues();
        IEnumerable<FilterModel> GetReportDeliveryOptions();
        IEnumerable<FilterModel> GetAddedToAccountOptions();
        IEnumerable<FilterModel> GetInvoiceRatings();
        IEnumerable<FilterModel> GetInvoiceEntityTypes();
        dynamic GetStaticData();
        IEnumerable<FilterModel> GetLabsForFilter();
    }
}
