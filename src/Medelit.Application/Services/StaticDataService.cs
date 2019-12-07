using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Medelit.Common;
using Medelit.Domain.Commands;
using Medelit.Domain.Core.Bus;
using Medelit.Domain.Interfaces;
using Medelit.Domain.Models;
using System.Collections.Generic;
using System.Linq;

namespace Medelit.Application
{
    public class StaticDataService : IStaticDataService
    {
        private readonly IAccountingCodeRepository _acodeRepository;

        public StaticDataService(          
            IAccountingCodeRepository acodeRepository        
            )
        {            
            _acodeRepository = acodeRepository;          
        }

        public dynamic GetProfessionalsForFitler()
        {
            return _acodeRepository.GetProfessionalsForFitler();
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

        public IEnumerable<ContractStatus> GetContractStatusOptions()
        {
            return _acodeRepository.GetContractStatus().ToList();
        }
        public IEnumerable<ApplicationMethod> GetApplicationMethods()
        {
            return _acodeRepository.GetApplicationMethods().ToList();
        }

        public IEnumerable<ApplicationMean> GetApplicationMeans()
        {
            return _acodeRepository.GetApplicationMeans().ToList();
        }

        public IEnumerable<DocumentListSent> GetDocumentListSents()
        {
            return _acodeRepository.GetDocumentListSents().ToList();
        }


        public IEnumerable<AccountingCode> GetAccountingCodes()
        {
            return _acodeRepository.GetAll().ToList();
        }

        public IEnumerable<CollaborationCode> GetCollaborationCodes()
        {
            return _acodeRepository.GetCollaborationCodes().ToList();
        }

        public IEnumerable<BookingStatus> GetBookingStatus()
        {
            return _acodeRepository.GetBookingStatus().ToList();
        }

        public IEnumerable<BookingType> GetBookingTypes()
        {
            return _acodeRepository.GetBookingTypes().ToList();
        }

        public IEnumerable<BuildingType> GetBuildingTypes()
        {
            return _acodeRepository.GetBuildingTypes().ToList();
        }

        public IEnumerable<City> GetCities()
        {
            return _acodeRepository.GetCities().ToList();
        }

        public IEnumerable<ContactMethod> GetContactMethods()
        {
            return _acodeRepository.GetContactMethods().ToList();
        }

        public IEnumerable<Country> GetCountries()
        {
            return _acodeRepository.GetCountries().ToList();
        }

        public IEnumerable<DiscountNetwork> GetDiscountNewtorks()
        {
            return _acodeRepository.GetDiscountNewtorks().ToList();
        }

        public IEnumerable<FilterModel> GetDurations()
        {
            return _acodeRepository.GetDurations().Select(x => new FilterModel { Id = x.Id, Value = $"{x.Value} {x.Unit}" }).ToList();
        }

        public IEnumerable<IERating> GetIERatings()
        {
            return _acodeRepository.GetIERatings().ToList();
        }

        public IEnumerable<IEType> GetIETypes()
        {
            return _acodeRepository.GetIETypes().ToList();
        }

        public IEnumerable<InvoiceStatus> GetInvoiceStatuses()
        {
            return _acodeRepository.GetInvoiceStatuses().ToList();
        }

        public dynamic GetLanguages()
        {
            return _acodeRepository.GetLanguages().ToList();
        }

        public IEnumerable<LeadCategory> GetLeadCategories()
        {
            return _acodeRepository.GetLeadCategories().ToList();
        }

        public IEnumerable<LeadSource> GetLeadSources()
        {
            return _acodeRepository.GetLeadSources().ToList();
        }

        public IEnumerable<LeadStatus> GetLeadStatuses()
        {
            return _acodeRepository.GetLeadStatuses().ToList();
        }

        public IEnumerable<PaymentMethods> GetPaymentMethods()
        {
            return _acodeRepository.GetPaymentMethods().ToList();
        }

        public IEnumerable<PaymentStatus> GetPaymentStatuses()
        {
            return _acodeRepository.GetPaymentStatuses().ToList();
        }

        public IEnumerable<Relationship> GetRelationships()
        {
            return _acodeRepository.GetRelationships().ToList();
        }

        public IEnumerable<Title> GetTitles()
        {
            return _acodeRepository.GetTitles().ToList();
        }

        public IEnumerable<FilterModel> GetVats()
        {
            return _acodeRepository.GetVats().Select(x => new FilterModel { Id = x.Id, Value = $"{Convert.ToInt64(x.Value)}{x.Unit}" });
        }

        public IEnumerable<VisitVenue> GetVisitVenues()
        {
            return _acodeRepository.GetVisitVenues().ToList();
        }

        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }
    }
}