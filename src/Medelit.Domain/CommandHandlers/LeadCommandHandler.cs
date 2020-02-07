using System;
using System.Threading;
using System.Threading.Tasks;
using Medelit.Domain.Commands;
using Medelit.Domain.Core.Bus;
using Medelit.Domain.Core.Notifications;
using Medelit.Domain.Interfaces;
using MediatR;
using AutoMapper;
using Microsoft.Extensions.Configuration;
using Medelit.Common;
using Medelit.Domain.Models;
using System.Linq;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;

namespace Medelit.Domain.CommandHandlers
{
    public class LeadCommandHandler : CommandHandler,
        IRequestHandler<SaveLeadCommand, bool>,
        IRequestHandler<UpdateLeadsStatusCommand, bool>,
        IRequestHandler<DeleteLeadsCommand, bool>,
        IRequestHandler<ConvertLeadToBookingCommand, bool>,
        IRequestHandler<LeadsBulkUploadCommand, bool>
    {
        private readonly IMapper _mapper;
        private readonly IMediatorHandler _bus;
        private readonly ILeadRepository _leadRepository;
        private readonly ICustomerRepository _customerRepository;
        private readonly IStaticDataRepository _staticRepository;


        public LeadCommandHandler(IMapper mapper,
            IUnitOfWork unitOfWork,
            IStaticDataRepository staticRepository,
            IMediatorHandler bus,
            IHttpContextAccessor httpContext,
            ILeadRepository leadRepository,
            ICustomerRepository customerRepository,

            INotificationHandler<DomainNotification> notifications)
            : base(bus, notifications, httpContext, unitOfWork)
        {
            _mapper = mapper;
            _staticRepository = staticRepository;
            _bus = bus;
            _leadRepository = leadRepository;
            _customerRepository = customerRepository;
        }

        public Task<bool> Handle(SaveLeadCommand request, CancellationToken cancellationToken)
        {
            try
            {
                bool commmitResult;
                var user = CurrentUser;
                if (request.Lead.Id > 0)
                {
                    var leadModel = _leadRepository.GetById(request.Lead.Id);
                    leadModel.TitleId = request.Lead.TitleId;
                    leadModel.SurName = request.Lead.SurName;
                    leadModel.Name = request.Lead.Name;
                    leadModel.LanguageId = request.Lead.LanguageId;
                    leadModel.MainPhone = request.Lead.MainPhone;
                    leadModel.MainPhoneOwner = request.Lead.MainPhoneOwner;
                    leadModel.ContactPhone = request.Lead.ContactPhone;
                    leadModel.Phone2 = request.Lead.Phone2;
                    leadModel.Phone2Owner = request.Lead.Phone2Owner;
                    leadModel.Phone3 = request.Lead.Phone3;
                    leadModel.Phone3Owner = request.Lead.Phone3Owner;
                    leadModel.Email = request.Lead.Email;
                    leadModel.Email2 = request.Lead.Email2;
                    leadModel.Fax = request.Lead.Fax;
                    leadModel.DateOfBirth = request.Lead.DateOfBirth;
                    leadModel.CountryOfBirthId = request.Lead.CountryOfBirthId;
                    leadModel.CountryId = request.Lead.CountryId;
                    leadModel.VisitRequestingPerson = request.Lead.VisitRequestingPerson;
                    leadModel.VisitRequestingPersonRelationId = request.Lead.VisitRequestingPersonRelationId;
                    leadModel.GPCode = request.Lead.GPCode;


                    leadModel.PreferredPaymentMethodId = request.Lead.PreferredPaymentMethodId;
                    leadModel.InsuranceCoverId = request.Lead.InsuranceCoverId;
                    leadModel.ListedDiscountNetworkId = request.Lead.ListedDiscountNetworkId;
                    leadModel.HaveDifferentIEId = request.Lead.HaveDifferentIEId;
                    leadModel.InvoiceEntityId = request.Lead.InvoiceEntityId;
                    leadModel.InvoicingNotes = request.Lead.Name;

                    leadModel.AddressStreetName = request.Lead.AddressStreetName;
                    leadModel.PostalCode = request.Lead.PostalCode;
                    leadModel.CountryId = request.Lead.CountryId;
                    leadModel.BuildingTypeId = request.Lead.BuildingTypeId;
                    leadModel.Buzzer = request.Lead.Buzzer;
                    leadModel.FlatNumber = request.Lead.FlatNumber;
                    leadModel.Floor = request.Lead.Floor;
                    leadModel.VisitVenueId = request.Lead.VisitVenueId;
                    leadModel.LeadStatusId = request.Lead.LeadStatusId;
                    leadModel.LeadSourceId = request.Lead.LeadSourceId;
                    leadModel.LeadCategoryId = request.Lead.LeadCategoryId;
                    leadModel.ContactMethodId = request.Lead.ContactMethodId;
                    leadModel.AddressNotes = request.Lead.AddressNotes;
                    leadModel.LeadDescription = request.Lead.LeadDescription;

                    leadModel.BankName = request.Lead.BankName;
                    leadModel.AccountNumber = request.Lead.AccountNumber;
                    leadModel.SortCode = request.Lead.SortCode;
                    leadModel.IBAN = request.Lead.IBAN;
                    leadModel.BlacklistId = request.Lead.BlacklistId;
                    if (request.FromCustomerId.HasValue && request.FromCustomerId.Value > 0)
                        leadModel.CustomerId = request.FromCustomerId.Value;

                    leadModel.UpdateDate = DateTime.UtcNow;
                    leadModel.UpdatedById = CurrentUser.Id;
                    leadModel.Services = request.Lead.Services;
                    _leadRepository.RemoveLeadServices(leadModel.Id);

                    _leadRepository.Update(leadModel);
                    commmitResult = Commit();
                    request.Lead = leadModel;

                }
                else
                {
                    var leadModel = request.Lead;
                    leadModel.CustomerId = request.FromCustomerId;
                    leadModel.AssignedToId = CurrentUser.Id;
                    leadModel.CreatedById = CurrentUser.Id;

                    _leadRepository.Add(leadModel);
                    commmitResult = Commit();
                    request.Lead = leadModel;
                }
                if (commmitResult)
                {
                    _bus.RaiseEvent(new DomainNotification(request.MessageType, null, request.Lead.Id));
                    return Task.FromResult(true);
                }
                else
                {
                    _bus.RaiseEvent(new DomainNotification(request.MessageType, MessageCodes.ERROR_OCCURED));
                    return Task.FromResult(false);
                }

            }
            catch (Exception ex)
            {
                return HandleException(request.MessageType, ex);
            }
        }
        public Task<bool> Handle(UpdateLeadsStatusCommand request, CancellationToken cancellationToken)
        {
            try
            {
                foreach (var fee in request.Leads)
                {
                    var feeModel = _leadRepository.GetById(fee.Id);
                    feeModel.Status = request.Status;
                    feeModel.UpdateDate = DateTime.UtcNow;
                    feeModel.UpdatedById = CurrentUser.Id;
                    _leadRepository.Update(feeModel);
                }
                if (Commit())
                {
                    _bus.RaiseEvent(new DomainNotification(request.MessageType, null, request.Leads));
                    return Task.FromResult(true);
                }
                else
                {
                    _bus.RaiseEvent(new DomainNotification(request.MessageType, MessageCodes.ERROR_OCCURED));
                    return Task.FromResult(false);
                }
            }
            catch (Exception ex)
            {
                return HandleException(request.MessageType, ex);
            }
        }

        public Task<bool> Handle(DeleteLeadsCommand request, CancellationToken cancellationToken)
        {
            try
            {
                foreach (var feeId in request.LeadIds)
                {
                    var feeModel = _leadRepository.GetById(feeId);
                    feeModel.Status = eRecordStatus.Deleted;
                    feeModel.DeletedAt = DateTime.UtcNow;
                    feeModel.DeletedById = CurrentUser.Id;
                    _leadRepository.Update(feeModel);
                }
                if (Commit())
                {
                    _bus.RaiseEvent(new DomainNotification(request.MessageType, null, request.LeadIds));
                    return Task.FromResult(true);
                }
                else
                {
                    _bus.RaiseEvent(new DomainNotification(request.MessageType, MessageCodes.ERROR_OCCURED));
                    return Task.FromResult(false);
                }
            }
            catch (Exception ex)
            {
                return HandleException(request.MessageType, ex);
            }
        }

        public Task<bool> Handle(ConvertLeadToBookingCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var lead = _leadRepository.GetWithInclude(request.LeadId);
                var customer = _mapper.Map<Customer>(lead);
                //customer.Services = _mapper.Map<ICollection<CustomerServiceRelation>>(lead.Services);

                customer.PaymentMethodId = lead.PreferredPaymentMethodId;
                customer.Email2 = lead.Email2;
                customer.HomeStreetName = lead.AddressStreetName;
                customer.VisitStreetName = lead.AddressStreetName;

                customer.HomeCityId = lead.CityId;
                customer.VisitCityId = lead.CityId;
                customer.CountryOfBirthId = lead.CountryId;
                customer.HomeCountryId = lead.CountryId;
                customer.VisitCountryId = lead.CountryId;
                customer.HomePostCode = lead.PostalCode;
                customer.VisitPostCode = lead.PostalCode;
                customer.ContactPhone = lead.ContactPhone;


                customer.LeadId = lead.Id;
                customer.Id = 0;
                customer.Services = new List<CustomerServiceRelation>();
                customer.CreatedById = CurrentUser.Id;
                _customerRepository.Add(customer);

                lead.ConvertDate = DateTime.UtcNow;
                lead.UpdatedById = CurrentUser.Id;
                _leadRepository.Update(lead);

                if (Commit())
                {
                    if (customer.Id > 0)
                    {
                        var services = _mapper.Map<ICollection<CustomerServiceRelation>>(lead.Services);
                        var newServices = new List<CustomerServiceRelation>();
                        foreach (var service in services)
                        {
                            newServices.Add(new CustomerServiceRelation
                            {
                                CustomerId = customer.Id,
                                ServiceId = service.ServiceId,
                                ProfessionalId = service.ProfessionalId,
                                PtFeeId = service.PtFeeId,
                                PTFeeA1 = service.PTFeeA1,
                                PTFeeA2 = service.PTFeeA2,
                                PROFeeId = service.PROFeeId,
                                PROFeeA1 = service.PROFeeA1,
                                PROFeeA2 = service.PROFeeA2
                            });
                        }
                        _customerRepository.SaveCustomerRelation(newServices);
                        _bus.SendCommand(new ConvertCustomerToBookingCommand { CustomerId = customer.Id }).Wait();
                    }

                    _bus.RaiseEvent(new DomainNotification(request.MessageType, null, request.LeadId));
                    return Task.FromResult(true);
                }
                else
                {
                    _bus.RaiseEvent(new DomainNotification(request.MessageType, MessageCodes.ERROR_OCCURED));
                    return Task.FromResult(false);
                }
            }
            catch (Exception ex)
            {
                return HandleException(request.MessageType, ex);
            }
        }

        public Task<bool> Handle(LeadsBulkUploadCommand request, CancellationToken cancellationToken)
        {
            try
            {
                if (request.LeadCSVData.Count() > 0)
                {

                    foreach (var obj in request.LeadCSVData)
                    {
                        var lead = new Lead();
                        lead.SurName = obj.SurName;
                        lead.TitleId = GetTitleId(obj.Title);
                        lead.Name = obj.Name;
                        lead.MainPhone = obj.MainPhone;
                        lead.MainPhoneOwner = obj.MainPhoneOwner;
                        lead.HaveDifferentIEId = 0;
                        lead.Phone2 = obj.Phone2;
                        lead.Phone3 = obj.Phone3;
                        lead.Phone2Owner = obj.Phone2Owner;
                        lead.Phone3Owner = obj.Phone3Owner;
                        lead.ContactPhone = obj.ContactPhone;
                        lead.VisitRequestingPerson = obj.VisitRequestingPerson;
                        lead.VisitRequestingPersonRelationId = GetRelationId(obj.VisitRequestingPersonRelation);
                        lead.Fax = obj.Fax;
                        lead.Email = obj.Email;
                        lead.LeadSourceId = GetLeadSourceId(obj.LeadSource);
                        lead.LeadStatusId = GetLeadStatusId(obj.LeadStatus);
                        lead.LanguageId = GetLanguageId(obj.Language);
                        lead.LeadCategoryId = GetLeadCategoryId(obj.LeadCategory);
                        lead.ContactMethodId = GetContactMethodId(obj.ContactMethod);
                        lead.DateOfBirth = GetDate(obj.DateOfBirth);
                        lead.CountryOfBirthId = GetCountryId(obj.CountryOfBirth);
                        lead.PreferredPaymentMethodId = GetPaymentmethodId(obj.PreferredPaymentMethod);
                        lead.InvoicingNotes = obj.InvoicingNotes;
                        lead.InsuranceCoverId = GetInsuranceCoverId(obj.InsuranceCover);
                        lead.ListedDiscountNetworkId = GetDiscountNetworkId(obj.ListedDiscountNetwork);
                        lead.Discount = obj.Discount;
                        lead.GPCode = obj.GPCode;
                        lead.AddressStreetName = obj.AddressStreetName;
                        lead.PostalCode = obj.PostalCode;
                        lead.CityId = GetCityId(obj.City);
                        lead.CountryId = GetCountryId(obj.Country);
                        lead.BuildingTypeId = GetBuildingTypeId(obj.Country);
                        lead.FlatNumber = obj.FlatNumber;
                        lead.Buzzer = obj.Buzzer;
                        lead.Floor = obj.Floor;
                        lead.VisitVenueId = GetVisitVenueId(obj.VisitVenue);
                        lead.AddressNotes = obj.AddressNotes;
                        lead.VisitVenueDetail = obj.VisitVenueDetail;
                        lead.LeadDescription = obj.LeadDescription;
                        lead.BankName = obj.BankName;
                        lead.AccountNumber = obj.AccountNumber;
                        lead.SortCode = obj.SortCode;
                        lead.IBAN = obj.IBAN;
                        lead.Email2 = obj.Email2;
                        lead.BlacklistId = GetBlackListId(obj.Blacklist);

                        lead.CreateDate = DateTime.Now;
                        lead.CreatedById = CurrentUser.Id;
                        lead.AssignedToId = CurrentUser.Id;

                        _leadRepository.Add(lead);
                    }
                    if (Commit())
                    {
                        _bus.RaiseEvent(new DomainNotification(request.MessageType, null, request.LeadCSVData.Count()));
                        return Task.FromResult(false);
                    }
                }
            }
            catch (Exception ex)
            {
                return HandleException(request.MessageType, ex);
            }
            _bus.RaiseEvent(new DomainNotification(request.MessageType, MessageCodes.ERROR_OCCURED));
            return Task.FromResult(false);

        }

        #region methods

        private short GetTitleId(string title)
        {
            if (string.IsNullOrEmpty(title))
                return default;

            var obj = _staticRepository.GetTitles().FirstOrDefault(x => x.Value.Equals(title.CLower(), StringComparison.InvariantCultureIgnoreCase));
            if (obj is null)
                return default;
            else
                return (short)obj.Id;
        }

        private short? GetRelationId(string relation)
        {
            if (string.IsNullOrEmpty(relation))
                return null;

            var obj = _staticRepository.GetRelationships().FirstOrDefault(x => x.Value.Equals(relation.CLower(), StringComparison.InvariantCultureIgnoreCase));
            if (obj is null)
                return null;
            else
                return (short?)obj.Id;
        }

        private short? GetLeadSourceId(string leadSource)
        {
            if (string.IsNullOrEmpty(leadSource))
                return null;

            var obj = _staticRepository.GetLeadSources().FirstOrDefault(x => x.Value.Equals(leadSource.CLower(), StringComparison.InvariantCultureIgnoreCase));
            if (obj is null)
                return null;
            else
                return (short?)obj.Id;
        }

        private short? GetLeadStatusId(string leadStatus)
        {
            if (string.IsNullOrEmpty(leadStatus))
                return null;

            var obj = _staticRepository.GetLeadStatuses().FirstOrDefault(x => x.Value.Equals(leadStatus.CLower(), StringComparison.InvariantCultureIgnoreCase));
            if (obj is null)
                return null;
            else
                return (short?)obj.Id;
        }

        private short? GetLanguageId(string lang)
        {
            if (string.IsNullOrEmpty(lang))
                return null;

            var obj = _staticRepository.GetLanguages().FirstOrDefault(x => x.Value.Equals(lang.CLower(), StringComparison.InvariantCultureIgnoreCase));
            if (obj is null)
                return null;
            else
                return (short?)obj.Id;
        }

        private short? GetLeadCategoryId(string leadCat)
        {
            if (string.IsNullOrEmpty(leadCat))
                return null;

            var obj = _staticRepository.GetLeadCategories().FirstOrDefault(x => x.Value.Equals(leadCat.CLower(), StringComparison.InvariantCultureIgnoreCase));
            if (obj is null)
                return null;
            else
                return (short?)obj.Id;
        }

        private short? GetContactMethodId(string contactMethod)
        {
            if (string.IsNullOrEmpty(contactMethod))
                return null;

            var obj = _staticRepository.GetContactMethods().FirstOrDefault(x => x.Value.Equals(contactMethod.CLower(), StringComparison.InvariantCultureIgnoreCase));
            if (obj is null)
                return null;
            else
                return (short?)obj.Id;
        }

        private DateTime? GetDate(string date)
        {
            if (string.IsNullOrEmpty(date))
                return null;
            if (DateTime.TryParseExact(date, "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out DateTime parsetDate))
            {
                return parsetDate;
            }
            return null;
        }

        private short GetCountryId(string country)
        {
            if (string.IsNullOrEmpty(country))
                return default;

            var obj = _staticRepository.GetCountries().FirstOrDefault(x => x.Value.Equals(country.CLower(), StringComparison.InvariantCultureIgnoreCase));
            if (obj is null)
                return default;
            else
                return obj.Id;
        }

        private short GetCityId(string city)
        {
            if (string.IsNullOrEmpty(city))
                return default;

            var obj = _staticRepository.GetCities().FirstOrDefault(x => x.Value.Equals(city.CLower(), StringComparison.InvariantCultureIgnoreCase));
            if (obj is null)
                return default;
            else
                return obj.Id;
        }

        private short? GetPaymentmethodId(string paymentMethod)
        {
            if (string.IsNullOrEmpty(paymentMethod))
                return null;

            var obj = _staticRepository.GetPaymentMethods().FirstOrDefault(x => x.Value.Equals(paymentMethod.CLower(), StringComparison.InvariantCultureIgnoreCase));
            if (obj is null)
                return null;
            else
                return (short?)obj.Id;
        }

        private short? GetInsuranceCoverId(string insuranceCover)
        {
            if (string.IsNullOrEmpty(insuranceCover))
                return null;


            if (insuranceCover.Equals("yes", StringComparison.InvariantCultureIgnoreCase))
                return 1;
            else
                return 0;
        }

        private short? GetDiscountNetworkId(string discountNetwork)
        {
            if (string.IsNullOrEmpty(discountNetwork))
                return null;

            var obj = _staticRepository.GetDiscountNewtorks().FirstOrDefault(x => x.Value.Equals(discountNetwork.CLower(), StringComparison.InvariantCultureIgnoreCase));
            if (obj is null)
                return null;
            else
                return (short?)obj.Id;
        }


        private short? GetBuildingTypeId(string buildingType)
        {
            if (string.IsNullOrEmpty(buildingType))
                return null;

            var obj = _staticRepository.GetBuildingTypes().FirstOrDefault(x => x.Value.Equals(buildingType.CLower(), StringComparison.InvariantCultureIgnoreCase));
            if (obj is null)
                return null;
            else
                return (short?)obj.Id;
        }

        private short? GetVisitVenueId(string visitVenue)
        {
            if (string.IsNullOrEmpty(visitVenue))
                return null;

            var obj = _staticRepository.GetVisitVenues().FirstOrDefault(x => x.Value.Equals(visitVenue.CLower(), StringComparison.InvariantCultureIgnoreCase));
            if (obj is null)
                return null;
            else
                return (short?)obj.Id;
        }

        private short? GetBlackListId(string blackList)
        {
            if (string.IsNullOrEmpty(blackList))
                return null;

            if (blackList.Equals("y", StringComparison.InvariantCultureIgnoreCase))
                return 1;
            else
                return 0;
        }

        #endregion methods

        public void Dispose()
        {

        }

    }
}