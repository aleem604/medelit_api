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
        IRequestHandler<DeleteInvoicesCommand, bool>
    {
        private readonly IMapper _mapper;
        private readonly IMediatorHandler _bus;
        private readonly IInvoiceRepository _invoiceRepository;
        private readonly ICustomerRepository _customerRepository;
        private readonly IConfiguration _config;


        public InvoiceCommandHandler(IMapper mapper,
            IUnitOfWork unitOfWork,
            IConfiguration config,
            IMediatorHandler bus,
            IHttpContextAccessor httpContext,
            IInvoiceRepository invoiceRepository,
            ICustomerRepository customerRepository,

            INotificationHandler<DomainNotification> notifications)
            : base(bus, notifications, httpContext, unitOfWork)
        {
            _mapper = mapper;
            _config = config;
            _bus = bus;
            _invoiceRepository = invoiceRepository;
            _customerRepository = customerRepository;
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


                    invoiceModel.Services = request.Invoice.Services;
                    _invoiceRepository.RemoveInvoiceServices(invoiceModel.Id);

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
                    var feeModel = request.Invoice;
                    _invoiceRepository.Add(feeModel);
                    commmitResult = Commit();
                    request.Invoice = feeModel;
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
                foreach (var fee in request.Invoices)
                {
                    var feeModel = _invoiceRepository.GetById(fee.Id);
                    feeModel.Status = request.Status;
                    feeModel.UpdateDate = DateTime.UtcNow;
                    _invoiceRepository.Update(feeModel);
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
                    //feeModel.DeletedById = 0;
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