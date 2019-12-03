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
        private readonly IBookingStatusRepository _bsRepository;
        private readonly IBookingTypeRepository _btypeRepository;
        private readonly IBuildingTypeRepository _buildingTypeRepository;
        private readonly IContactMethodRepository _contactMethodRepository;
        private readonly ICityRepository _cityRepositry;
        private readonly ICountryRepository _countryRepository;
        private readonly IDiscountNetworkRepository _discountNetworkRepository;
        private readonly IDurationRepository _durationRepository;
        private readonly IIERatingRepository _iERatingRepository;
        private readonly IIETypeRepository _iETypeRepository;
        private readonly IInvoiceStatusRepository _invoiceStatusRepository;
        private readonly ILanguageRepository _languageRepository;
        private readonly ILeadCategoryRepository _leadCategoryRepository;
        private readonly ILeadSourceRepository _leadSourceRepository;
        private readonly ILeadStatusRepository _leadStatusRepository;
        private readonly ITitleRepository _titleRepository;
        private readonly IPaymentMethodsRepository _paymentMethodsRepository;
        private readonly IPaymentStatusRepository _paymentStatusRepository;
        private readonly IRelationshipRepository _relationashipRepository;
        private readonly IVatRepository _vatRepository;
        private readonly IVisitVenueRepository _visitVenueRepository;

        public StaticDataService(
            IAccountingCodeRepository acodeRepository,
            IBookingStatusRepository   bsRepository,
            IBookingTypeRepository  btypeRepository,
            IBuildingTypeRepository buildingTypeRepository,
            IContactMethodRepository contactMethodRepository,
            ICityRepository cityRepository,
            ICountryRepository  countryRepository,
            IDiscountNetworkRepository discountNetworkRepository,
            IDurationRepository durationRepository,
            IIERatingRepository iERatingRepository,
            IIETypeRepository iETypeRepository,
            IInvoiceStatusRepository invoiceStatusRepository,
            ILanguageRepository languageRepository,
            ILeadCategoryRepository leadCategoryRepository,
            ILeadSourceRepository leadSourceRepository,
            ILeadStatusRepository leadStatusRepository,
            ITitleRepository titleRepository,
            IPaymentMethodsRepository paymentMethodsRepository,
            IPaymentStatusRepository paymentStatusRepository,
            IRelationshipRepository relationashipRepository,
            IVatRepository vatRepository,
            IVisitVenueRepository visitVenueRepository
                       
            )
        {
            _acodeRepository = acodeRepository;
            _bsRepository = bsRepository;
            _btypeRepository = btypeRepository;
            _buildingTypeRepository = buildingTypeRepository;
            _contactMethodRepository = contactMethodRepository;
            _cityRepositry = cityRepository;
            _countryRepository = countryRepository;
            _discountNetworkRepository = discountNetworkRepository;
            _durationRepository = durationRepository;
            _iERatingRepository = iERatingRepository;
            _iETypeRepository = iETypeRepository;
            _invoiceStatusRepository = invoiceStatusRepository;
            _languageRepository = languageRepository;
            _leadCategoryRepository = leadCategoryRepository;
            _leadSourceRepository = leadSourceRepository;
            _leadStatusRepository = leadStatusRepository;
            _titleRepository = titleRepository;
            _paymentMethodsRepository = paymentMethodsRepository;
            _paymentStatusRepository = paymentStatusRepository;
            _relationashipRepository = relationashipRepository;
            _vatRepository = vatRepository;
            _visitVenueRepository = visitVenueRepository;
        }

        public IEnumerable<AccountingCode> GetAccountingCodes()
        {
            return _acodeRepository.GetAll().ToList();
        }

        public IEnumerable<BookingStatus> GetBookingStatus()
        {
            return _bsRepository.GetAll().ToList();
        }

        public IEnumerable<BookingType> GetBookingTypes()
        {
            return _btypeRepository.GetAll().ToList();
        }

        public IEnumerable<BuildingType> GetBuildingTypes()
        {
            return _buildingTypeRepository.GetAll().ToList();
        }

        public IEnumerable<City> GetCities()
        {
            return _cityRepositry.GetAll().ToList();
        }

        public IEnumerable<ContactMethod> GetContactMethods()
        {
            return _contactMethodRepository.GetAll().ToList();
        }

        public IEnumerable<Country> GetCountries()
        {
            return _countryRepository.GetAll().ToList();
        }

        public IEnumerable<DiscountNetwork> GetDiscountNewtorks()
        {
            return _discountNetworkRepository.GetAll().ToList();
        }

        public IEnumerable<Duration> GetDurations()
        {
            return _durationRepository.GetAll().ToList();
        }

        public IEnumerable<IERating> GetIERatings()
        {
            return _iERatingRepository.GetAll().ToList();
        }

        public IEnumerable<IEType> GetIETypes()
        {
            return _iETypeRepository.GetAll().ToList();
        }

        public IEnumerable<InvoiceStatus> GetInvoiceStatuses()
        {
            return _invoiceStatusRepository.GetAll().ToList();
        }

        public dynamic GetLanguages()
        {
            return _languageRepository.GetAll().Select(x=> new { Id= x.Id, Value = x.Name }).ToList();
        }

        public IEnumerable<LeadCategory> GetLeadCategories()
        {
            return _leadCategoryRepository.GetAll().ToList();
        }

        public IEnumerable<LeadSource> GetLeadSources()
        {
            return _leadSourceRepository.GetAll().ToList();
        }

        public IEnumerable<LeadStatus> GetLeadStatuses()
        {
            return _leadStatusRepository.GetAll().ToList();
        }

        public IEnumerable<PaymentMethods> GetPaymentMethods()
        {
            return _paymentMethodsRepository.GetAll().ToList();
        }

        public IEnumerable<PaymentStatus> GetPaymentStatuses()
        {
            return _paymentStatusRepository.GetAll().ToList();
        }

        public IEnumerable<Relationship> GetRelationships()
        {
            return _relationashipRepository.GetAll().ToList();
        }

        public IEnumerable<Title> GetTitles()
        {
            return _titleRepository.GetAll().ToList();
        }

        public IEnumerable<Vat> GetVats()
        {
            return _vatRepository.GetAll().ToList();
        }

        public IEnumerable<VisitVenue> GetVisitVenues()
        {
            return _visitVenueRepository.GetAll().ToList();
        }

        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }
    }
}