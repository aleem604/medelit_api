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
using Microsoft.AspNetCore.Http;

namespace Medelit.Domain.CommandHandlers
{
    public class InvoiceEntityCommandHandler : CommandHandler,
        IRequestHandler<SaveInvoiceEntityCommand, bool>,
        IRequestHandler<UpdateInvoiceEntitiesStatusCommand, bool>,
        IRequestHandler<DeleteInvoiceEntitiesCommand, bool>
    {
        private readonly IMapper _mapper;
        private readonly IMediatorHandler _bus;
        private readonly IInvoiceEntityRepository _invoiceEntityRepository;
        private readonly ICustomerRepository _customerRepository;
        private readonly IConfiguration _config;


        public InvoiceEntityCommandHandler(IMapper mapper,
            IUnitOfWork unitOfWork,
            IConfiguration config,
            IMediatorHandler bus,
            IHttpContextAccessor httpContext,
            IInvoiceEntityRepository invoiceEntityRepository,
            ICustomerRepository customerRepository,

            INotificationHandler<DomainNotification> notifications)
            : base(bus, notifications, httpContext, unitOfWork)
        {
            _mapper = mapper;
            _config = config;
            _bus = bus;
            _invoiceEntityRepository = invoiceEntityRepository;
            _customerRepository = customerRepository;
        }

        public Task<bool> Handle(SaveInvoiceEntityCommand request, CancellationToken cancellationToken)
        {
            try
            {
                bool commmitResult;
                if (request.Entity.Id > 0)
                {
                    var ieModel = _invoiceEntityRepository.GetById(request.Entity.Id);

                    ieModel.Name = request.Entity.Name;
                    ieModel.MainPhoneNumber = request.Entity.MainPhoneNumber;
                    ieModel.MainPhoneNumberOwner = request.Entity.MainPhoneNumberOwner;

                    ieModel.Phone2 = request.Entity.Phone2;
                    ieModel.Phone2Owner = request.Entity.Phone2Owner;
                    ieModel.Phone3 = request.Entity.Phone3;
                    ieModel.Phone3Owner = request.Entity.Phone3Owner;
                    ieModel.Email = request.Entity.Email;
                    ieModel.Email2 = request.Entity.Email2;
                    ieModel.RatingId = request.Entity.RatingId;
                    ieModel.RelationshipWithCustomerId = request.Entity.RelationshipWithCustomerId;
                    ieModel.IETypeId = request.Entity.IETypeId;

                    ieModel.Fax = request.Entity.Fax;
                    ieModel.DateOfBirth = request.Entity.DateOfBirth;
                    ieModel.CountryOfBirthId = request.Entity.CountryOfBirthId;
                    ieModel.BillingAddress = request.Entity.BillingAddress;
                    ieModel.MailingAddress = request.Entity.MailingAddress;
                    ieModel.BillingPostCode = request.Entity.BillingPostCode;
                    ieModel.MailingPostCode = request.Entity.MailingPostCode;
                    ieModel.BillingCity = request.Entity.BillingCity;
                    ieModel.MailingCity = request.Entity.MailingCity;
                    ieModel.BillingCountryId = request.Entity.BillingCountryId;
                    ieModel.MailingCountryId = request.Entity.MailingCountryId;
                    ieModel.Description = request.Entity.Description;
                    ieModel.VatNumber = request.Entity.VatNumber;
                    ieModel.PaymentMethodId = request.Entity.PaymentMethodId;
                    ieModel.Bank = request.Entity.Bank;
                    ieModel.AccountNumber = request.Entity.AccountNumber;
                    ieModel.SortCode = request.Entity.SortCode;
                    ieModel.IBAN = request.Entity.IBAN;
                    ieModel.InsuranceCoverId = request.Entity.InsuranceCoverId;
                    ieModel.InvoicingNotes = request.Entity.InvoicingNotes;
                    ieModel.DiscountNetworkId = request.Entity.DiscountNetworkId;
                    ieModel.PersonOfReference = request.Entity.PersonOfReference;
                    ieModel.PersonOfReferenceEmail = request.Entity.PersonOfReferenceEmail;
                    ieModel.PersonOfReferencePhone = request.Entity.PersonOfReferencePhone;
                    ieModel.BlackListId = request.Entity.BlackListId;
                    ieModel.ContractedId = request.Entity.ContractedId;
                    ieModel.UpdateDate = DateTime.UtcNow;
                    ieModel.UpdatedById = CurrentUser.Id;
                    ieModel.AssignedToId = CurrentUser.Id;

                    _invoiceEntityRepository.Update(ieModel);
                    commmitResult = Commit();
                    request.Entity = ieModel;
                }
                else
                {
                    var ieModel = request.Entity;
                    ieModel.CreatedById = CurrentUser.Id;
                    ieModel.AssignedToId = CurrentUser.Id;
                    _invoiceEntityRepository.Add(ieModel);
                    commmitResult = Commit();
                    if (commmitResult && ieModel.Id > 0)
                    {
                        var id = ieModel.Id;
                        ieModel.IENumber = $"IE{id.ToString().PadLeft(6, '0')}";
                        ieModel.UpdatedById = CurrentUser.Id;
                        _invoiceEntityRepository.Update(ieModel);
                        commmitResult = Commit();
                    }
                    request.Entity = ieModel;
                }
                if (commmitResult)
                {
                    _bus.RaiseEvent(new DomainNotification(request.MessageType, null, request.Entity));
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

        public Task<bool> Handle(UpdateInvoiceEntitiesStatusCommand request, CancellationToken cancellationToken)
        {
            try
            {
                foreach (var fee in request.Entities)
                {
                    var feeModel = _invoiceEntityRepository.GetById(fee.Id);
                    feeModel.Status = request.Status;
                    feeModel.UpdateDate = DateTime.UtcNow;
                    feeModel.UpdatedById = CurrentUser.Id;
                    _invoiceEntityRepository.Update(feeModel);
                }
                if (Commit())
                {
                    _bus.RaiseEvent(new DomainNotification(request.MessageType, null, request.Entities));
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

        public Task<bool> Handle(DeleteInvoiceEntitiesCommand request, CancellationToken cancellationToken)
        {
            try
            {
                foreach (var feeId in request.InvoieEntityIds)
                {
                    var feeModel = _invoiceEntityRepository.GetById(feeId);
                    feeModel.Status = eRecordStatus.Deleted;
                    feeModel.DeletedAt = DateTime.UtcNow;
                    feeModel.DeletedById = CurrentUser.Id;
                    _invoiceEntityRepository.Update(feeModel);
                }
                if (Commit())
                {
                    _bus.RaiseEvent(new DomainNotification(request.MessageType, null, request.InvoieEntityIds));
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

        public void Dispose()
        {}

    }
}