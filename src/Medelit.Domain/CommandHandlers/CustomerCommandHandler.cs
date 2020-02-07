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
    public class CustomerCommandHandler : CommandHandler,
        IRequestHandler<SaveCustomerCommand, bool>,
        IRequestHandler<UpdateCustomersStatusCommand, bool>,
        IRequestHandler<DeleteCustomersCommand, bool>,
        IRequestHandler<ConvertCustomerToBookingCommand, bool>
    {
        private readonly IMapper _mapper;
        private readonly IMediatorHandler _bus;
        private readonly ICustomerRepository _customerRepository;
        private readonly IBookingRepository _bookingRepository;
        private readonly IConfiguration _config;


        public CustomerCommandHandler(IMapper mapper,
            IUnitOfWork unitOfWork,
            IConfiguration config,
            IMediatorHandler bus,
            IHttpContextAccessor httpContext,
            ICustomerRepository customerRepository,
            IBookingRepository bookingRepository,

            INotificationHandler<DomainNotification> notifications)
            : base(bus, notifications, httpContext, unitOfWork)
        {
            _mapper = mapper;
            _config = config;
            _bus = bus;
            _customerRepository = customerRepository;
            _bookingRepository = bookingRepository;
        }

        public Task<bool> Handle(SaveCustomerCommand request, CancellationToken cancellationToken)
        {
            try
            {
                bool commmitResult;
                if (request.Customer.Id > 0)
                {
                    var customerModel = _customerRepository.GetById(request.Customer.Id);
                    customerModel.TitleId = request.Customer.TitleId;
                    customerModel.SurName = request.Customer.SurName;
                    customerModel.Name = request.Customer.Name;


                    customerModel.LanguageId = request.Customer.LanguageId;
                    customerModel.MainPhone = request.Customer.MainPhone;
                    customerModel.MainPhoneOwner = request.Customer.MainPhoneOwner;
                    customerModel.ContactPhone = request.Customer.ContactPhone;
                    customerModel.Phone2 = request.Customer.Phone2;
                    customerModel.Phone2Owner = request.Customer.Phone2Owner;
                    customerModel.Phone3 = request.Customer.Phone3;
                    customerModel.Phone3Owner = request.Customer.Phone3Owner;
                    customerModel.Email = request.Customer.Email;
                    customerModel.Email2 = request.Customer.Email2;
                    customerModel.Fax = request.Customer.Fax;
                    customerModel.DateOfBirth = request.Customer.DateOfBirth;
                    customerModel.HomeCountryId = request.Customer.HomeCountryId;
                    customerModel.VisitCountryId = request.Customer.VisitCountryId;
                    customerModel.VisitRequestingPerson = request.Customer.VisitRequestingPerson;
                    customerModel.VisitRequestingPersonRelationId = request.Customer.VisitRequestingPersonRelationId;
                    customerModel.GPCode = request.Customer.GPCode;

                    customerModel.PaymentMethodId = request.Customer.PaymentMethodId;
                    customerModel.ListedDiscountNetworkId = request.Customer.ListedDiscountNetworkId;
                    customerModel.HaveDifferentIEId = request.Customer.HaveDifferentIEId;
                    customerModel.InvoiceEntityId = request.Customer.InvoiceEntityId;
                    customerModel.InvoicingNotes = request.Customer.Name;

                    customerModel.HomeStreetName = request.Customer.HomeStreetName;
                    customerModel.VisitStreetName = request.Customer.VisitStreetName;
                    customerModel.HomePostCode = request.Customer.HomePostCode;
                    customerModel.VisitPostCode = request.Customer.VisitPostCode;
                    customerModel.HomeCountryId = request.Customer.HomeCountryId;
                    customerModel.VisitCountryId = request.Customer.VisitCountryId;
                    customerModel.BuildingTypeId = request.Customer.BuildingTypeId;
                    customerModel.Buzzer = request.Customer.Buzzer;
                    customerModel.FlatNumber = request.Customer.FlatNumber;
                    customerModel.Floor = request.Customer.Floor;
                    customerModel.VisitVenueId = request.Customer.VisitVenueId;
                    customerModel.ContactMethodId = request.Customer.ContactMethodId;
                    customerModel.AddressNotes = request.Customer.AddressNotes;

                    customerModel.BankName = request.Customer.BankName;
                    customerModel.AccountNumber = request.Customer.AccountNumber;
                    customerModel.IBAN = request.Customer.IBAN;
                    customerModel.SortCode = request.Customer.SortCode;
                    customerModel.BlacklistId = request.Customer.BlacklistId;


                    customerModel.UpdateDate = DateTime.UtcNow;
                    customerModel.UpdatedById = CurrentUser.Id;
                    _customerRepository.Update(customerModel);
                    commmitResult = Commit();
                    request.Customer = customerModel;
                }
                else
                {
                    var feeModel = request.Customer;
                    feeModel.CreatedById = CurrentUser.Id;
                    _customerRepository.Add(feeModel);
                    commmitResult = Commit();
                    request.Customer = feeModel;
                }
                if (commmitResult)
                {
                    _bus.RaiseEvent(new DomainNotification(request.MessageType, null, request.Customer));
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

        public Task<bool> Handle(UpdateCustomersStatusCommand request, CancellationToken cancellationToken)
        {
            try
            {
                foreach (var fee in request.Customers)
                {
                    var feeModel = _customerRepository.GetById(fee.Id);
                    feeModel.Status = request.Status;
                    feeModel.UpdateDate = DateTime.UtcNow;
                    feeModel.UpdatedById = CurrentUser.Id;
                    _customerRepository.Update(feeModel);
                }
                if (Commit())
                {
                    _bus.RaiseEvent(new DomainNotification(request.MessageType, null, request.Customers));
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

        public Task<bool> Handle(DeleteCustomersCommand request, CancellationToken cancellationToken)
        {
            try
            {
                foreach (var feeId in request.CustomerIds)
                {
                    var feeModel = _customerRepository.GetById(feeId);
                    feeModel.Status = eRecordStatus.Deleted;
                    feeModel.DeletedAt = DateTime.UtcNow;
                    feeModel.DeletedById = CurrentUser.Id;
                    _customerRepository.Update(feeModel);
                }
                if (Commit())
                {
                    _bus.RaiseEvent(new DomainNotification(request.MessageType, null, request.CustomerIds));
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

        public Task<bool> Handle(ConvertCustomerToBookingCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var customer = _customerRepository.GetById(request.CustomerId);
                var services = customer.Services;
                long lastBookingId = 0;
                foreach (var service in services)
                {
                    var booking = _mapper.Map<Booking>(customer);

                    booking.InvoiceEntityId = customer.InvoiceEntityId;
                    booking.CustomerId = customer.Id;

                    booking.ServiceId = service.ServiceId;
                    booking.ProfessionalId = service.ProfessionalId;
                    booking.PtFeeId = service.PtFeeId.Value;
                    booking.PtFee = service.IsPtFeeA1 == 1 ? service.PTFeeA1 : service.PTFeeA2;
                    booking.ProFeeId = service.PROFeeId.Value;
                    booking.ProFee = service.IsProFeeA1 == 1 ? service.PROFeeA1 : service.PROFeeA2;

                    booking.Name = _bookingRepository.GetBookingName(customer.Name, customer.SurName);

                    booking.DateOfBirth = customer.DateOfBirth;
                    booking.PhoneNumber = customer.MainPhone;
                    booking.PaymentMethodId = customer.PaymentMethodId;
                    booking.Email2 = customer.Email2;
                    booking.HomeStreetName = customer.HomeStreetName;
                    booking.VisitLanguageId = customer.LanguageId;
                    booking.BuildingTypeId = customer.BuildingTypeId;
                    booking.PaymentConcludedId = 0;
                    booking.CCAuthorizationId = 0;
                    booking.CashConfirmationMailId = 0;
                    booking.BankTransfterReceiptId = 0;
                    booking.PaymentStatusId = 4;
                    booking.PaymentConcludedId = 0;
                    booking.ImToProId = 0;
                    booking.MailToPtId = 0;
                    booking.AddToAccountingId = 2;
                    booking.NHSOrPrivateId = 0;
                    booking.PtCalledForAppointmentId = 0;
                    booking.ProAvailabilityAskedId = 0;

                    if (customer.DateOfBirth.HasValue)
                        booking.PatientAge = (short?)((DateTime.Now - customer.DateOfBirth).Value.Days / 365.25);


                    booking.Id = 0;
                    booking.BookingDate = DateTime.UtcNow;
                    booking.CreatedById = CurrentUser.Id;
                    _bookingRepository.Add(booking);
                    if (Commit())
                        lastBookingId = booking.Id;
                }

                _bus.RaiseEvent(new DomainNotification(request.MessageType, null, lastBookingId));
                return Task.FromResult(true);

            }
            catch (Exception ex)
            {
                return HandleException(request.MessageType, ex);
            }
        }

        public void Dispose()
        {

        }

    }
}