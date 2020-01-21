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
                    invoiceModel.InvoiceEntityId = request.Invoice.InvoiceEntityId;
                    invoiceModel.PatientDateOfBirth = request.Invoice.PatientDateOfBirth;
                    invoiceModel.StatusId = request.Invoice.StatusId;
                    invoiceModel.DueDate = request.Invoice.DueDate;
                    invoiceModel.InvoiceDate = request.Invoice.InvoiceDate;
                    invoiceModel.InvoiceDeliveryDate = request.Invoice.InvoiceDeliveryDate;
                    invoiceModel.InvoiceSentByEmailId = request.Invoice.InvoiceSentByEmailId;
                    invoiceModel.InvoiceSentByMailId = request.Invoice.InvoiceSentByMailId;

                    invoiceModel.IEBillingAddress = request.Invoice.IEBillingAddress;
                    invoiceModel.IEBillingPostCode = request.Invoice.IEBillingPostCode;
                    invoiceModel.IEBillingCityId = request.Invoice.IEBillingCityId;
                    invoiceModel.IEBillingCountryId = request.Invoice.IEBillingCountryId;

                    invoiceModel.MailingAddress = request.Invoice.MailingAddress;
                    invoiceModel.MailingPostCode = request.Invoice.MailingPostCode;
                    invoiceModel.MailingCityId = request.Invoice.MailingCityId;
                    invoiceModel.MailingCountryId = request.Invoice.MailingCountryId;

                    invoiceModel.PaymentMethodId = request.Invoice.PaymentMethodId;
                    invoiceModel.InsuranceCoverId = request.Invoice.InsuranceCoverId;
                    invoiceModel.InvoiceNotes = request.Invoice.InvoiceNotes;
                    invoiceModel.InvoiceDiagnosis = request.Invoice.InvoiceDiagnosis;
                    invoiceModel.TotalInvoice = UpdateInvoiceTotals(request.Invoice.Id);

                    invoiceModel.UpdateDate = DateTime.UtcNow;
                    invoiceModel.UpdatedById = CurrentUser.Id;

                    _invoiceRepository.Update(invoiceModel);
                    
                    commmitResult = Commit();
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
                foreach (var feeId in request.InvoiceIds)
                {
                    var feeModel = _invoiceRepository.GetById(feeId);
                    feeModel.Status = eRecordStatus.Deleted;
                    feeModel.DeletedAt = DateTime.UtcNow;
                    feeModel.DeletedById = CurrentUser.Id;
                    _invoiceRepository.Update(feeModel);
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
                    var updateInvoice = _invoiceRepository.GetById(request.InvoiceId);
                    updateInvoice.TotalInvoice = UpdateInvoiceTotals(request.InvoiceId);
                    updateInvoice.UpdatedById = CurrentUser.Id;
                    updateInvoice.UpdateDate = DateTime.UtcNow;

                    _invoiceRepository.Update(updateInvoice);
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
                var booking = _bookingRepository.GetById(request.BookingId);
                var invoiceEntity = booking.InvoiceEntityId.HasValue ? _ieRepository.GetById((long)booking.InvoiceEntityId) : new InvoiceEntity();
                var customer = _customerRepository.GetAll().FirstOrDefault(x => x.Id == booking.CustomerId);
                var invoice = new Invoice();
                invoice.Subject = booking.InvoiceNumber ?? customer.Name;
                invoice.InvoiceEntityId = booking.InvoiceEntityId;
                invoice.CustomerId = booking.CustomerId;
                invoice.InvoiceNumber = $"{DateTime.Now.ToString("ddMM/yyyy")} PROFORMA";
                invoice.DueDate = booking.InvoiceDueDate ?? DateTime.Now;
                invoice.InvoiceDate = booking.VisitStartDate;
                //invoice.TaxCodeId = 
                invoice.StatusId = booking.BookingStatusId;
                invoice.PaymentDue = booking.InvoiceDueDate;
                invoice.PaymentDue = DateTime.Now;
                invoice.InvoiceSentByEmailId = 0;
                invoice.InvoiceSentByMailId = 0;
                invoice.PaymentMethodId = booking.PaymentMethodId;
                invoice.PatientDateOfBirth = booking.DateOfBirth;

                invoice.IEBillingAddress = invoice.IEBillingAddress;
                invoice.MailingAddress = invoiceEntity.MailingAddress;

                invoice.IEBillingPostCode = invoiceEntity.BillingPostCode;
                invoice.MailingPostCode = invoiceEntity.MailingPostCode;

                invoice.IEBillingCityId = invoiceEntity.BillingCityId;
                invoice.MailingCityId = invoiceEntity.MailingCityId;

                invoice.IEBillingCountryId = invoiceEntity.BillingCountryId;
                invoice.MailingCountryId = invoiceEntity.MailingCountryId;

                invoice.InvoiceNotes = booking.InvoicingNotes;
                invoice.InsuranceCoverId = booking.InsuranceCoverId;
                invoice.InvoiceDiagnosis = booking.Diagnosis;
                invoice.InvoiceDiagnosis = booking.Diagnosis;
                invoice.DateOfVisit = booking.VisitStartDate;
                invoice.InvoiceDescription = customer.Name;

                invoice.PaymentArrivalDate = booking.PaymentArrivalDate;
                invoice.ProInvoiceDate = booking.InvoiceDueDate;

                invoice.CreateDate = DateTime.UtcNow;
                invoice.CreatedById = CurrentUser.Id;
                invoice.AssignedToId = CurrentUser.Id;

                _invoiceRepository.Add(invoice);
                if (Commit())
                {
                    if (invoice.Id > 0)
                    {
                        var invoiceBookingModel = _invoiceRepository.AddBookingToInvoice(booking.Id, invoice.Id);
                        var newInvoice = _invoiceRepository.GetById(invoice.Id);
                        newInvoice.InvoiceNumber = $"{invoice.Id.ToString().PadLeft(5, '0')}/{DateTime.Now.ToString("ddMM/yyyy")} PROFORMA";
                        newInvoice.TotalInvoice = UpdateInvoiceTotals(newInvoice.Id);
                        newInvoice.UpdateDate = DateTime.UtcNow;
                        newInvoice.UpdatedById = CurrentUser.Id;

                        _invoiceRepository.Update(newInvoice);
                        Commit();
                    }

                    _bus.RaiseEvent(new DomainNotification(request.MessageType, null, booking));
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
                var ib = _invoiceRepository.GetInvoiceBookings().FirstOrDefault(x => x.Id == request.InvoiceBookingId);
                var invoiceBookings = _invoiceRepository.GetInvoiceBookings().Where(x => x.InvoiceId == ib.InvoiceId && x.Id != request.InvoiceBookingId).ToList();
                var invoice = _invoiceRepository.GetById(ib.InvoiceId);

                decimal? totals = null;
                foreach (var booking in invoiceBookings)
                {
                    var bookingObj = _bookingRepository.GetAll().Where(x => x.Id == booking.BookingId).Select((s) => new { s.Id, s.PtFee, s.QuantityHours, s.TaxAmount, s.TaxType, s.SubTotal, s.GrossTotal }).FirstOrDefault();
                    if (bookingObj != null)
                    {
                        var subTotal = GetSubTotal(bookingObj.PtFee, bookingObj.QuantityHours);
                        var taxAmount = GetCusotmerTaxAmount(subTotal, bookingObj.TaxType);
                        totals += subTotal + taxAmount;
                    }
                }
                invoice.TotalInvoice = totals;
                _invoiceRepository.DeleteInvoiceBooking(ib);

                invoice.UpdateDate = DateTime.UtcNow;
                invoice.UpdatedById = CurrentUser.Id;
                _invoiceRepository.Update(invoice);

                _bus.RaiseEvent(new DomainNotification(request.MessageType, null, Commit()));
                return Task.FromResult(true);
            }
            catch (Exception ex)
            {
                return HandleException(request.MessageType, ex);
            }
        }
      
        public decimal? UpdateInvoiceTotals(long invoiceId)
        {
            var invoiceBookings = _invoiceRepository.GetInvoiceBookings().Where(x => x.InvoiceId == invoiceId).ToList();
            decimal? totals = 0;
            foreach (var booking in invoiceBookings)
            {
                var bookingObj = _bookingRepository.GetAll().Where(x => x.Id == booking.BookingId).Select((s) => new { s.Id, s.PtFee, s.QuantityHours, s.TaxAmount, s.TaxType, s.SubTotal, s.GrossTotal }).FirstOrDefault();
                if (bookingObj != null)
                {
                    var subTotal = GetSubTotal(bookingObj.PtFee, bookingObj.QuantityHours);
                    var taxAmount = GetCusotmerTaxAmount(subTotal, bookingObj.TaxType);
                    totals += subTotal + taxAmount;
                }
            }
            return totals;
        }

        private decimal? GetSubTotal(decimal? ptFee, short? quantityHours)
        {
            if (ptFee.HasValue && quantityHours.HasValue)
                return ptFee.Value * quantityHours.Value;
            return null;
        }

        private decimal? GetCusotmerTaxAmount(decimal? subTotal, short? taxType)
        {
            if (subTotal.HasValue && taxType.HasValue)
                return subTotal.Value * taxType.Value * (decimal)0.01;
            return null;
        }

        public void Dispose()
        {

        }

    }
}