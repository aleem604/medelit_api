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
        IRequestHandler<ConvertLeadToBookingCommand, bool>
    {
        private readonly IMapper _mapper;
        private readonly IMediatorHandler _bus;
        private readonly ILeadRepository _leadRepository;
        private readonly ICustomerRepository _customerRepository;
        private readonly IConfiguration _config;


        public LeadCommandHandler(IMapper mapper,
            IUnitOfWork unitOfWork,
            IConfiguration config,
            IMediatorHandler bus,
            IHttpContextAccessor httpContext,
            ILeadRepository leadRepository,
            ICustomerRepository customerRepository,

            INotificationHandler<DomainNotification> notifications)
            : base(bus, notifications, httpContext, unitOfWork)
        {
            _mapper = mapper;
            _config = config;
            _bus = bus;
            _leadRepository = leadRepository;
            _customerRepository = customerRepository;
        }

        public Task<bool> Handle(SaveLeadCommand request, CancellationToken cancellationToken)
        {
            try
            {
                bool commmitResult;
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
                    leadModel.Services = request.Lead.Services;
                    _leadRepository.RemoveLeadServices(leadModel.Id);

                    _leadRepository.Update(leadModel);
                    commmitResult = Commit();
                    request.Lead = leadModel;

                    //var allLeads = _feeRepository.GetAll();
                    //foreach (var fee in allLeads)
                    //{

                    //        fee.LeadCode = fee.LeadTypeId == eLeadType.PTLead ? $"FP{fee.Id.ToString().PadLeft(6, '0')}" : $"FS{fee.Id.ToString().PadLeft(6, '0')}";
                    //        fee.UpdateDate = DateTime.UtcNow;
                    //        _feeRepository.Update(fee);

                    //}
                    //Commit();
                }
                else
                {
                    var leadModel = request.Lead;
                    leadModel.CustomerId = request.FromCustomerId;

                    _leadRepository.Add(leadModel);
                    commmitResult = Commit();
                    request.Lead = leadModel;
                }
                if (commmitResult)
                {
                    _bus.RaiseEvent(new DomainNotification(request.MessageType, null, request.Lead));
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
                    //feeModel.DeletedById = 0;
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


                customer.LeadId = lead.Id;
                customer.Id = 0;
                customer.Services = new List<CustomerServiceRelation>();

                _customerRepository.Add(customer);

                lead.ConvertDate = DateTime.UtcNow;
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
                                PTFeeId = service.PTFeeId,
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


        private Task<bool> HandleException(string messageType, Exception ex)
        {
            _bus.RaiseEvent(new DomainNotification(messageType, ex.Message));
            return Task.FromResult(false);
        }
        public void Dispose()
        {

        }

    }
}