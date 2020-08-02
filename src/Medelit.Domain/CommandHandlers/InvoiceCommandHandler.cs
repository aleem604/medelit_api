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
    public class InvoiceCommandHandler : CommandHandler,
        IRequestHandler<SaveInvoiceCommand, bool>,
        IRequestHandler<UpdateInvoicesStatusCommand, bool>,
        IRequestHandler<DeleteInvoicesCommand, bool>,
        IRequestHandler<AddBookingToInvoiceCommand, bool>,
        IRequestHandler<CreateInvoiceFromBookingCommand, bool>,
        IRequestHandler<DeleteInvoiceBookingCommand, bool>
    {
        private readonly IMapper _mapper;
        private readonly IMediatorHandler _bus;
        private readonly IInvoiceRepository _invoiceRepository;
        private readonly ICustomerRepository _customerRepository;
        private readonly IBookingRepository _bookingRepository;
        private readonly IInvoiceEntityRepository _ieRepository;
        private readonly IConfiguration _config;


        public InvoiceCommandHandler(IMapper mapper,
            IUnitOfWork unitOfWork,
            IConfiguration config,
            IMediatorHandler bus,
            IHttpContextAccessor httpContext,
            IInvoiceRepository invoiceRepository,
            ICustomerRepository customerRepository,
            IBookingRepository bookingRepository,
            IInvoiceEntityRepository ieRepository,

            INotificationHandler<DomainNotification> notifications)
            : base(bus, notifications, httpContext, unitOfWork)
        {
            _mapper = mapper;
            _config = config;
            _bus = bus;
            _invoiceRepository = invoiceRepository;
            _customerRepository = customerRepository;
            _bookingRepository = bookingRepository;
            _ieRepository = ieRepository;
        }

        public Task<bool> Handle(SaveInvoiceCommand request, CancellationToken cancellationToken)
        {
            try
            {
                bool commmitResult;
                if (request.Invoice.Id > 0)
                {
                    var invoiceModel = _invoiceRepository.GetById(request.Invoice.Id);

                    invoiceModel.UpdateDate = DateTime.UtcNow;
                    invoiceModel.Subject = request.Invoice.Subject;
                    invoiceModel.InvoiceNumber = request.Invoice.InvoiceNumber;
                    invoiceModel.CustomerId = request.Invoice.CustomerId;
                    invoiceModel.InvoiceEntityId = request.Invoice.InvoiceEntityId;
                    invoiceModel.DateOfVisit = request.Invoice.DateOfVisit;
                    invoiceModel.PatientDateOfBirth = request.Invoice.PatientDateOfBirth;
                    invoiceModel.StatusId = request.Invoice.StatusId;
                    invoiceModel.DueDate = request.Invoice.DueDate;
                    invoiceModel.InvoiceDate = request.Invoice.InvoiceDate;
                    invoiceModel.InvoiceDeliveryDate = request.Invoice.InvoiceDeliveryDate;
                    invoiceModel.InvoiceSentByEmailId = request.Invoice.InvoiceSentByEmailId;
                    invoiceModel.InvoiceSentByMailId = request.Invoice.InvoiceSentByMailId;

                    invoiceModel.IEBillingAddress = request.Invoice.IEBillingAddress;
                    invoiceModel.IEBillingPostCode = request.Invoice.IEBillingPostCode;
                    invoiceModel.IEBillingCity = request.Invoice.IEBillingCity;
                    invoiceModel.IEBillingCountryId = request.Invoice.IEBillingCountryId;

                    invoiceModel.MailingAddress = request.Invoice.MailingAddress;
                    invoiceModel.MailingPostCode = request.Invoice.MailingPostCode;
                    invoiceModel.MailingCity = request.Invoice.MailingCity;
                    invoiceModel.MailingCountryId = request.Invoice.MailingCountryId;

                    invoiceModel.PaymentMethodId = request.Invoice.PaymentMethodId;
                    invoiceModel.PaymentArrivalDate = request.Invoice.PaymentArrivalDate;
                    invoiceModel.PaymentDueDate = request.Invoice.PaymentDueDate;
                    invoiceModel.Discount = request.Invoice.Discount;
                    invoiceModel.InsuranceCoverId = request.Invoice.InsuranceCoverId;
                    invoiceModel.InvoiceNotes = request.Invoice.InvoiceNotes;
                    invoiceModel.InvoiceDiagnosis = request.Invoice.InvoiceDiagnosis;
                    invoiceModel.InvoiceDescription = request.Invoice.InvoiceDescription;
                    invoiceModel.TermsAndConditions = request.Invoice.TermsAndConditions;
                    invoiceModel.ItemNameOnInvoice = request.Invoice.ItemNameOnInvoice;
                    invoiceModel.UpdateDate = request.Invoice.UpdateDate;

                    _invoiceRepository.Update(invoiceModel);

                    commmitResult = Commit();
                    _invoiceRepository.UpdateInvoiceTotal(invoiceModel.Id);
                    request.Invoice = invoiceModel;

                    //var allInvoices = _feeRepository.GetAll();
                    //foreach (var fee in allInvoices)
                    //{

                    //        fee.InvoiceCode = fee.InvoiceTypeId == eInvoiceType.PTInvoice ? $"FP{fee.Id.ToString().PadLeft(6, '0')}" : $"FS{fee.Id.ToString().PadLeft(6, '0')}";
                    //        fee.UpdateDate = DateTime.UtcNow;
                    //        _feeRepository.Update(fee);

                    //}
                    //Commit();
                }
                else
                {
                    var invoiceModel = request.Invoice;
                    invoiceModel.AssignedToId = CurrentUser.Id;
                    invoiceModel.CreatedById = CurrentUser.Id;
                    invoiceModel.CreateDate = DateTime.UtcNow;

                    _invoiceRepository.Add(invoiceModel);
                    commmitResult = Commit();
                    request.Invoice = invoiceModel;

                }
                if (commmitResult)
                {
                    _invoiceRepository.UpdateInvoiceTotal(request.Invoice.Id);
                    _bus.RaiseEvent(new DomainNotification(request.MessageType, null, request.Invoice));
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

        public Task<bool> Handle(UpdateInvoicesStatusCommand request, CancellationToken cancellationToken)
        {
            try
            {
                foreach (var invoice in request.Invoices)
                {
                    var invoiceModel = _invoiceRepository.GetById(invoice.Id);
                    invoiceModel.Status = request.Status;
                    invoiceModel.UpdateDate = DateTime.UtcNow;
                    invoiceModel.UpdatedById = CurrentUser.Id;

                    _invoiceRepository.Update(invoiceModel);
                }
                if (Commit())
                {
                    _bus.RaiseEvent(new DomainNotification(request.MessageType, null, request.Invoices));
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

        public Task<bool> Handle(DeleteInvoicesCommand request, CancellationToken cancellationToken)
        {
            try
            {
                foreach (var invoiceId in request.InvoiceIds)
                {
                    var bookings = _invoiceRepository.GetInvoiceBookings().Where(i => i.InvoiceId == invoiceId).Select(b => b.BookingId).ToList();
                    foreach (var booking in bookings)
                    {
                        var b = _bookingRepository.GetById(booking);
                        b.InvoiceId = null;
                        b.UpdateDate = DateTime.UtcNow;
                        b.UpdatedById = CurrentUser.Id;
                        _bookingRepository.Update(b);
                    }                   
                    _invoiceRepository.Remove(invoiceId);
                }
                if (Commit())
                {
                    _bus.RaiseEvent(new DomainNotification(request.MessageType, null, request.InvoiceIds));
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

        public Task<bool> Handle(AddBookingToInvoiceCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var booking = _bookingRepository.GetById(request.BookingId);
                var invoice = _invoiceRepository.GetById(request.InvoiceId);
                var invoiceBooking = _invoiceRepository.GetInvoiceBookings().Where(x => x.InvoiceId == request.InvoiceId && x.BookingId == request.BookingId).Count();
                if (invoiceBooking > 0)
                {
                    _bus.RaiseEvent(new DomainNotification(request.MessageType, "Booking alredy exist int the selected invoice."));
                    return Task.FromResult(false);
                }

                if (booking is null || invoice is null)
                {
                    _bus.RaiseEvent(new DomainNotification(request.MessageType, MessageCodes.API_DATA_INVALID));
                    return Task.FromResult(false);
                }

                var invoiceBookingModel = _invoiceRepository.AddBookingToInvoice(request.BookingId, request.InvoiceId);

                if (invoiceBookingModel.Id > 0)
                {
                    booking.InvoiceId = request.InvoiceId;
                    booking.UpdateDate = DateTime.UtcNow;
                    booking.UpdatedById = CurrentUser.Id;
                    Commit();

                    _bus.RaiseEvent(new DomainNotification(request.MessageType, null, invoiceBookingModel));
                    return Task.FromResult(false);
                }
                else
                {
                    _bus.RaiseEvent(new DomainNotification(request.MessageType, MessageCodes.API_DATA_INVALID));
                    return Task.FromResult(true);
                }
            }
            catch (Exception ex)
            {
                return HandleException(request.MessageType, ex);
            }
        }

        public Task<bool> Handle(CreateInvoiceFromBookingCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var booking = _bookingRepository.GetAll().Where(x => x.Id == request.BookingId).Include(x => x.Service).FirstOrDefault();
                var invoiceEntity = booking.InvoiceEntityId.HasValue ? _ieRepository.GetById((long)booking.InvoiceEntityId) : new InvoiceEntity();
                var customer = _customerRepository.GetAll().FirstOrDefault(x => x.Id == booking.CustomerId);
                var invoice = new Invoice();
                invoice.Subject = booking.InvoiceEntityId.HasValue ? invoiceEntity.Name : $"{customer.SurName} {customer.Name}";
                invoice.InvoiceEntityId = booking.InvoiceEntityId;
                invoice.CustomerId = booking.CustomerId;
                invoice.InvoiceNumber = $"{DateTime.Now.ToString("yyyy")} PROFORMA";
                invoice.DueDate = GetInvoiceDueDate(booking);
                invoice.InvoiceDate = booking.VisitStartDate;
                //invoice.TaxCodeId = 
                invoice.StatusId = booking.BookingStatusId;
                invoice.PaymentDueDate = GetInvoicePaymentDueDate(booking);

                invoice.InvoiceSentByEmailId = 0;
                invoice.InvoiceSentByMailId = 0;
                invoice.PaymentMethodId = booking.PaymentMethodId;
                invoice.PatientDateOfBirth = booking.InvoiceEntityId.HasValue ? invoiceEntity.DateOfBirth : customer.DateOfBirth;

                //invoice.IEBillingAddress = invoiceEntity.BillingAddress ?? customer.HomeStreetName;
                //invoice.MailingAddress = invoiceEntity.MailingAddress ?? customer.HomeStreetName;

                invoice.IEBillingAddress = booking.HomeStreetName;
                invoice.MailingAddress = booking.HomeStreetName;

                //invoice.IEBillingPostCode = invoiceEntity.BillingPostCode ?? customer.HomePostCode;
                //invoice.MailingPostCode = invoiceEntity.MailingPostCode ?? customer.HomePostCode;

                invoice.IEBillingPostCode = booking.HomePostCode;
                invoice.MailingPostCode = booking.HomePostCode;

                //invoice.IEBillingCity = invoiceEntity.BillingCity ?? customer.HomeCity;
                //invoice.MailingCity = invoiceEntity.MailingCity ?? customer.HomeCity; 

                invoice.IEBillingCity = booking.HomeCity;
                invoice.MailingCity = booking.HomeCity;

                //invoice.IEBillingCountryId = invoiceEntity.BillingCountryId ?? customer.HomeCountryId;
                //invoice.MailingCountryId = invoiceEntity.MailingCountryId ?? customer.HomeCountryId;

                invoice.IEBillingCountryId = booking.HomeCountryId;
                invoice.MailingCountryId = booking.HomeCountryId;

                invoice.InvoiceNotes = booking.NotesOnPayment;
                invoice.InsuranceCoverId = booking.InsuranceCoverId;
                invoice.InvoiceDiagnosis = booking.Diagnosis;
                invoice.InvoiceDiagnosis = booking.Diagnosis;
                invoice.DateOfVisit = booking.VisitStartDate;
                invoice.TermsAndConditions = "Invoice not subject to VAT";
                invoice.InvoiceDescription = customer.Name;
                invoice.ItemNameOnInvoice = string.Concat(booking.Name, " ", booking.Service.Name);

                invoice.PaymentArrivalDate = booking.PaymentArrivalDate;
                invoice.ProInvoiceDate = booking.InvoiceDueDate;
                invoice.InvoiceDeliveryDate = DateTime.Now;
                invoice.StatusId = (short)eInvoiceStatus.Proforma;

                invoice.CreateDate = booking.UpdateDate ?? DateTime.UtcNow;
                invoice.CreatedById = CurrentUser.Id;
                invoice.AssignedToId = CurrentUser.Id;

                _invoiceRepository.Add(invoice);
                if (Commit())
                {
                    if (invoice.Id > 0)
                    {
                        booking.InvoiceId = invoice.Id;
                        booking.UpdateDate = DateTime.Now;
                        booking.UpdatedById = CurrentUser.Id;
                        _bookingRepository.Update(booking);

                        var invoiceBookingModel = _invoiceRepository.AddBookingToInvoice(booking.Id, invoice.Id);
                        var newInvoice = _invoiceRepository.GetById(invoice.Id);
                        newInvoice.InvoiceNumber = _invoiceRepository.GetProformaInoviceNumber(invoice.Id);
                        //  invoice.Subject = _invoiceRepository.GetProformaInoviceNumber(invoice.Id);
                        newInvoice.IsProforma = true;

                        _invoiceRepository.Update(newInvoice);
                        Commit();
                        _invoiceRepository.UpdateInvoiceTotal(newInvoice.Id);
                    }

                    _bus.RaiseEvent(new DomainNotification(request.MessageType, null, invoice.Id));
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

        public Task<bool> Handle(DeleteInvoiceBookingCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var ib = _invoiceRepository.GetInvoiceBookings().FirstOrDefault(x => x.InvoiceId == request.InvoiceId && x.BookingId == request.BookingId);
               
                _invoiceRepository.DeleteInvoiceBooking(ib);
                if (Commit())
                {
                    //var invoiceObj = _invoiceRepository.GetById(invoice.Id);
                    //invoiceObj.TotalInvoice = UpdateInvoiceTotals(invoiceObj.Id);
                    //_invoiceRepository.Update(invoiceObj);
                    _invoiceRepository.UpdateInvoiceTotal(request.InvoiceId);
                }

                _bus.RaiseEvent(new DomainNotification(request.MessageType, null, Commit()));
                return Task.FromResult(true);
            }
            catch (Exception ex)
            {
                return HandleException(request.MessageType, ex);
            }
        }

        private DateTime? GetInvoiceDueDate(Booking booking)
        {
            var invoiceDueDate = booking.VisitStartDate;

            if (booking.PaymentMethodId == (short?)ePaymentMethods.Insurance)
            {
                if (booking.VisitStartDate.HasValue)
                    invoiceDueDate = booking.VisitStartDate.Value.AddDays(30);
            }

            return invoiceDueDate;
        }

        private DateTime? GetInvoicePaymentDueDate(Booking booking)
        {
            var paymentDueDate = booking.VisitStartDate;

            var paymentMethod = booking.PaymentMethodId;
            if (booking.PaymentMethodId == (short?)ePaymentMethods.Insurance)
            {
                if (booking.VisitStartDate.HasValue)
                    paymentDueDate = booking.VisitStartDate.Value.AddDays(30);
            }
            if (!paymentMethod.HasValue || Convert.ToInt16(booking.BookingStatusId) == (short)eBookingStatus.CancelledAfterAcceptance)
            {
                paymentDueDate = null;
            }
            return paymentDueDate;
        }

        public void Dispose()
        {

        }

    }
}