using Medelit.Common;
using Medelit.Domain.Models;
using System.Collections.Generic;
using System.Linq;

namespace Medelit.Domain.Interfaces
{
    public interface IStaticDataRepository : IRepository<StaticData>
    {
        IQueryable<FilterModel> GetCustomersForImportFilter();
        IQueryable<FilterModel> GetCustomersForFilter();
        IQueryable<FilterModel> GetInvoicesForFilter();
        dynamic GePROFeesForFilter();
        IQueryable<FilterModel> GetAccountingCodes();
        IQueryable<FilterModel> GetAddedToAccountOptions();
        IQueryable<FilterModel> GetApplicationMeans();
        IQueryable<FilterModel> GetApplicationMethods();
        IQueryable<FilterModel> GetBookingStatus();
        IQueryable<FilterModel> GetBookingTypes();
        IQueryable<FilterModel> GetBuildingTypes();
        IQueryable<City> GetCities();
        IQueryable<FilterModel> GetCollaborationCodes();
        IQueryable<FilterModel> GetContactMethods();
        IQueryable<FilterModel> GetContractStatus();
        IQueryable<Country> GetCountries();
        IQueryable<FilterModel> GetDiscountNewtorks();
        IQueryable<FilterModel> GetDocumentListSents();
        IQueryable<FilterModel> GetDurations();
        IQueryable<FilterModel> GetFieldsForFilter();
        IQueryable<FilterModel> GetIERatings();
        IQueryable<FilterModel> GetIETypes();
        IQueryable<FilterModel> GetInvoiceEntities();
        IQueryable<FilterModel> GetInvoiceStatuses();
        IEnumerable<FilterModel> GetLabs();
        IQueryable<FilterModel> GetLanguages();
        IQueryable<FilterModel> GetLeadCategories();
        IQueryable<FilterModel> GetLeadSources();
        IQueryable<FilterModel> GetLeadStatuses();
        IQueryable<FilterModel> GetPaymentMethods();
        IQueryable<FilterModel> GetPaymentStatuses();
        dynamic GetProfessionalsForFitler(long? serviceId);
        dynamic GetPTFeesForFilter();
        IQueryable<FilterModel> GetRelationships();
        IQueryable<FilterModel> GetReportDeliveryOptions();
        dynamic GetServicesForFitler();
        dynamic GetProfessionalsWithFeesForFitler(long? serviceId);
        IQueryable<StaticData> GetStaticData();
        IQueryable<FilterModel> GetSubCategoriesForFilter(IEnumerable<FilterModel> fields);
        IQueryable<FilterModel> GetTitles();
        IQueryable<FilterModel> GetVats();
        IQueryable<FilterModel> GetVisitVenues();
        dynamic GetAccountInfo();
    }
}