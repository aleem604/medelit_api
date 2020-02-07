using System;
using Medelit.Common;
using Medelit.Domain.Interfaces;
using Medelit.Domain.Models;
using System.Collections.Generic;
using System.Linq;

namespace Medelit.Application
{
    public class StaticDataService : IStaticDataService
    {
        private readonly IStaticDataRepository _acodeRepository;

        public StaticDataService(
            IStaticDataRepository acodeRepository
            )
        {
            _acodeRepository = acodeRepository;
        }

        public IEnumerable<FilterModel> GetCustomersForImportFilter()
        {
            return _acodeRepository.GetCustomersForImportFilter().ToList();
        }
        public IEnumerable<FilterModel> GetInvoicesForFilter()
        {
            return _acodeRepository.GetInvoicesForFilter().ToList();
        }
        public IEnumerable<FilterModel> GetInvoiceEntities()
        {
            return _acodeRepository.GetInvoiceEntities().ToList();
        }
        public dynamic GetServicesForFitler()
        {
            return _acodeRepository.GetServicesForFitler();
        }

        public dynamic GetProfessionalsWithFeesForFitler(long? serviceId)
        {
            return _acodeRepository.GetProfessionalsWithFeesForFitler(serviceId);
        }

        public dynamic GetProfessionalsForFitler(long? serviceId)
        {
            return _acodeRepository.GetProfessionalsForFitler(serviceId);
        }

        public dynamic GePTFeesForFilter()
        {
            return _acodeRepository.GetPTFeesForFilter();
        }

        public dynamic GetPROFeesForFilter()
        {
            return _acodeRepository.GePROFeesForFilter();
        }

        public IEnumerable<FilterModel> GetFieldsForFilter()
        {
            return _acodeRepository.GetFieldsForFilter().ToList();
        }
        public IEnumerable<FilterModel> GetSubCategoriesForFilter()
        {
            return _acodeRepository.GetSubCategoriesForFilter().ToList();
        }

        public IEnumerable<FilterModel> GetContractStatusOptions()
        {
            return _acodeRepository.GetStaticData().Where(x => x.ContractStatus != null).Select(x => new FilterModel { Id = x.Id, Value = x.ContractStatus }).ToList();
        }
        public IEnumerable<FilterModel> GetApplicationMethods()
        {
            return _acodeRepository.GetStaticData().Where(x => x.ApplicationMethods != null).Select(x => new FilterModel { Id = x.Id, Value = x.ApplicationMethods }).ToList();
        }

        public IEnumerable<FilterModel> GetApplicationMeans()
        {
            return _acodeRepository.GetStaticData().Where(x => x.ApplicationMeans != null).Select(x => new FilterModel { Id = x.Id, Value = x.ApplicationMeans }).ToList();
        }

        public IEnumerable<FilterModel> GetDocumentListSents()
        {
            return _acodeRepository.GetStaticData().Where(x => x.DocumentListSentOptions != null).Select(x => new FilterModel { Id = x.Id, Value = x.DocumentListSentOptions }).ToList();
        }


        public IEnumerable<FilterModel> GetAccountingCodes()
        {
            return _acodeRepository.GetStaticData().Where(x => x.AccountingCodes != null).Select(x => new FilterModel { Id = x.Id, Value = x.AccountingCodes }).ToList();
        }

        public IEnumerable<FilterModel> GetCollaborationCodes()
        {
            return _acodeRepository.GetStaticData().Where(x => x.CollaborationCodes != null).Select(x => new FilterModel { Id = x.Id, Value = x.CollaborationCodes }).ToList();
        }

        public IEnumerable<FilterModel> GetBookingStatus()
        {
            return _acodeRepository.GetStaticData().Where(x => x.BookingStatus != null).Select(x => new FilterModel { Id = x.Id, Value = x.BookingStatus }).ToList();
        }

        public IEnumerable<FilterModel> GetBookingTypes()
        {
            return _acodeRepository.GetStaticData().Where(x => x.BookingTypes != null).Select(x => new FilterModel { Id = x.Id, Value = x.BookingTypes }).ToList();
        }

        public IEnumerable<FilterModel> GetBuildingTypes()
        {
            return _acodeRepository.GetStaticData().Where(x => x.BuildingTypes != null).Select(x => new FilterModel { Id = x.Id, Value = x.BuildingTypes }).ToList();
        }

        public IEnumerable<City> GetCities()
        {
            return _acodeRepository.GetCities().ToList();
        }

        public IEnumerable<FilterModel> GetContactMethods()
        {
            return _acodeRepository.GetStaticData().Where(x => x.ContactMethods != null).Select(x => new FilterModel { Id = x.Id, Value = x.ContactMethods }).ToList();
        }

        public IEnumerable<Country> GetCountries()
        {
            return _acodeRepository.GetCountries().ToList();
        }

        public IEnumerable<FilterModel> GetDiscountNewtorks()
        {
            return _acodeRepository.GetStaticData().Where(x => x.DiscountNetworks != null).Select(x => new FilterModel { Id = x.Id, Value = x.DiscountNetworks }).ToList();
        }

        public IEnumerable<FilterModel> GetDurations()
        {
            return _acodeRepository.GetStaticData().Where(x => x.Durations != null).Select(x => new FilterModel { Id = x.Id, Value = $"{x.Durations} {x.DurationUnits}" }).ToList();
        }

        public IEnumerable<FilterModel> GetIERatings()
        {
            return _acodeRepository.GetStaticData().Where(x => x.IERatings != null).Select(x => new FilterModel { Id = x.Id, Value = x.IERatings }).ToList();
        }

        public IEnumerable<FilterModel> GetIETypes()
        {
            return _acodeRepository.GetStaticData().Where(x => x.IETypes != null).Select(x => new FilterModel { Id = x.Id, Value = x.IETypes }).ToList();
        }

        public IEnumerable<FilterModel> GetInvoiceStatuses()
        {
            return _acodeRepository.GetStaticData().Where(x => x.InvoiceStatus != null).Select(x => new FilterModel { Id = x.Id, Value = x.InvoiceStatus }).ToList();
        }

        public dynamic GetLanguages()
        {
            return _acodeRepository.GetLanguages().ToList();
        }

        public IEnumerable<FilterModel> GetLeadCategories()
        {
            return _acodeRepository.GetStaticData().Select(x => new FilterModel { Id = x.Id, Value = x.LeadCategories }).Where(x => x.Value != null).ToList();
        }

        public IEnumerable<FilterModel> GetLeadSources()
        {
            return _acodeRepository.GetStaticData().Where(x => x.LeadSources != null).Select(x => new FilterModel { Id = x.Id, Value = x.LeadSources }).ToList();
        }

        public IEnumerable<FilterModel> GetLeadStatuses()
        {
            return _acodeRepository.GetStaticData().Where(x => x.LeadStatus != null).Select(x => new FilterModel { Id = x.Id, Value = x.LeadStatus }).ToList();
        }

        public IEnumerable<FilterModel> GetPaymentMethods()
        {
            return _acodeRepository.GetStaticData().Where(x => x.PaymentMethods != null).Select(x => new FilterModel { Id = x.Id, Value = x.PaymentMethods }).ToList();
        }

        public IEnumerable<FilterModel> GetPaymentStatuses()
        {
            return _acodeRepository.GetStaticData().Where(x => x.PaymentStatus != null).Select(x => new FilterModel { Id = x.Id, Value = x.PaymentStatus }).ToList();
        }

        public IEnumerable<FilterModel> GetRelationships()
        {
            return _acodeRepository.GetStaticData().Where(x => x.Relationships != null).Select(x => new FilterModel { Id = x.Id, Value = x.Relationships }).ToList();
        }

        public IEnumerable<FilterModel> GetTitles()
        {
            return _acodeRepository.GetStaticData().Where(x => x.Titles != null).Select(x => new FilterModel { Id = x.Id, Value = x.Titles }).ToList();
        }

        public IEnumerable<FilterModel> GetVats()
        {
            return _acodeRepository.GetStaticData().Where(x => x.Vats != null).Select(x => new FilterModel { Id = x.Id, Value = $"{Convert.ToInt64(x.Vats)}{x.VatUnit}" });
        }

        public IEnumerable<FilterModel> GetVisitVenues()
        {
            return _acodeRepository.GetStaticData().Where(x => x.VisitVenues != null).Select(x => new FilterModel { Id = x.Id, Value = x.VisitVenues }).ToList();
        }

        public IEnumerable<FilterModel> GetReportDeliveryOptions()
        {
            return _acodeRepository.GetStaticData().Where(x => x.ReportDeliveryOptions != null).Select(x => new FilterModel { Id = x.Id, Value = x.ReportDeliveryOptions }).ToList();
        }

        public IEnumerable<FilterModel> GetAddedToAccountOptions()
        {
            return _acodeRepository.GetStaticData().Where(x => x.AddToAccountOptions != null).Select(x => new FilterModel { Id = x.Id, Value = x.AddToAccountOptions }).ToList();
        }

        public IEnumerable<FilterModel> GetInvoiceRatings()
        {
            return _acodeRepository.GetStaticData().Where(x=>x.IERatings != null).Select(x => new FilterModel { Id = x.Id, Value = x.IERatings }).ToList();
        }

        public IEnumerable<FilterModel> GetInvoiceEntityTypes()
        {
            return _acodeRepository.GetStaticData().Where(x=>x.IETypes != null).Select(x => new FilterModel { Id = x.Id, Value = x.IETypes }).ToList();
        }
         public dynamic GetStaticData()
        {
            return _acodeRepository.GetStaticData().ToList();
        }

        public IEnumerable<FilterModel> GetLabsForFilter()
        {
            return _acodeRepository.GetLabs().ToList();
        }

        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }
    }
}