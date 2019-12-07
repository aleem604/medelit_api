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

namespace Medelit.Domain.CommandHandlers
{
    public class ProfessionalCommandHandler : CommandHandler,
        IRequestHandler<SaveProfessionalCommand, bool>,
        IRequestHandler<UpdateProfessionalsStatusCommand, bool>,
        IRequestHandler<DeleteProfessionalsCommand, bool>
    {
        private readonly IMapper _mapper;
        private readonly IMediatorHandler _bus;
        private readonly IProfessionalRepository _professionalRepository;
        private readonly IConfiguration _config;


        public ProfessionalCommandHandler(IMapper mapper,
            IUnitOfWork unitOfWork,
            IConfiguration config,
            IMediatorHandler bus,
            IHttpContextAccessor httpContext,
            IProfessionalRepository professionalRepository,

            INotificationHandler<DomainNotification> notifications)
            : base(bus, notifications, httpContext, unitOfWork)
        {
            _mapper = mapper;
            _config = config;
            _bus = bus;
            _professionalRepository = professionalRepository;
        }

        public Task<bool> Handle(SaveProfessionalCommand request, CancellationToken cancellationToken)
        {
            try
            {
                bool commitResult = false;
                if (request.Model.Id > 0)
                {
                    var req = request.Model;
                    var proModel = _professionalRepository.GetById(request.Model.Id);

                    proModel.TitleId = req.TitleId;
                    proModel.Name = req.Name;
                    proModel.Email = req.Email;
                    proModel.Email2 = req.Email2;
                    proModel.Telephone = req.Telephone;
                    proModel.AccountingCodeId = req.AccountingCodeId;
                    proModel.Website = req.Website;
                    proModel.MobilePhone = req.MobilePhone;
                    proModel.HomePhone = req.HomePhone;
                    proModel.Fax = req.Fax;
                    proModel.CoverMap = req.CoverMap;
                    proModel.StreetName = req.StreetName;
                    proModel.CityId = req.CityId;
                    proModel.PostCode = req.PostCode;
                    proModel.CountryId = req.CountryId;
                    proModel.Description = req.Description;
                    proModel.ClinicStreetName = req.ClinicStreetName;
                    proModel.ClinicPostCode = req.ClinicPostCode;
                    proModel.ClinicCityId = req.ClinicCityId;
                    proModel.ClinicPhoneNumber = req.ClinicPhoneNumber;
                    proModel.CompanyNumber = req.CompanyNumber;
                    proModel.InvoicingNotes = req.InvoicingNotes;
                    proModel.Bank = req.Bank;
                    proModel.Branch = req.Branch;
                    proModel.AccountName = req.AccountName;
                    proModel.AccountNumber = req.AccountNumber;
                    proModel.SortCode = req.SortCode;
                    proModel.ContractDate = req.ContractDate;
                    proModel.ContractEndDate = req.ContractEndDate;
                    proModel.WorkPlace = req.WorkPlace;
                    proModel.ColleagueReferring = req.ColleagueReferring;
                    proModel.InsuranceExpiryDate = req.InsuranceExpiryDate;
                    proModel.ActiveCollaborationId = req.ActiveCollaborationId;
                    proModel.ClinicAgreement = req.ClinicAgreement;
                    proModel.ApplicationMethodId = req.ApplicationMethodId;
                    proModel.ApplicationMeansId = req.ApplicationMeansId;
                    proModel.FirstContactDate = req.FirstContactDate;
                    proModel.LastContactDate = req.LastContactDate;
                    proModel.ContractStatusId = req.ContractStatusId;
                    proModel.DocumentListSentId = req.DocumentListSentId;
                    proModel.CalendarActivation = req.CalendarActivation;
                    proModel.ProOnlineCV = req.ProOnlineCV;
                    proModel.ProtaxCode = req.ProtaxCode;
                    proModel.ProfessionalLangs = req.ProfessionalLangs;
                    proModel.UpdateDate = DateTime.UtcNow;
                    _professionalRepository.DeleteLangs(req.Id);
                    _professionalRepository.Update(proModel);

                    //var allFees = _professionalRepository.GetAll();
                    //foreach (var fee in allFees)
                    //{
                    //    if (string.IsNullOrEmpty(fee.FeeCode))
                    //    {
                    //        fee.FeeCode = $"FP{fee.Id.ToString().PadLeft(6, '0')}";
                    //        fee.UpdateDate = DateTime.UtcNow;
                    //        _feeRepository.Update(fee);
                    //    }
                    //}
                    commitResult = Commit();
                }
                else
                {
                    var proModel = request.Model;
                    proModel.CreateDate = DateTime.UtcNow;
                    _professionalRepository.Add(proModel);
                    commitResult = Commit();
                    if (commitResult && proModel.Id > 0)
                    {
                        var id = proModel.Id;
                        proModel.Code = $"V{id.ToString().PadLeft(6, '0')}";
                        _professionalRepository.Update(proModel);
                        commitResult = Commit();
                    }
                    request.Model = proModel;
                }
                if (commitResult)
                {
                    _bus.RaiseEvent(new DomainNotification(request.MessageType, null, request.Model));
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
        public Task<bool> Handle(UpdateProfessionalsStatusCommand request, CancellationToken cancellationToken)
        {
            try
            {
                foreach (var fee in request.Professionals)
                {
                    var feeModel = _professionalRepository.GetById(fee.Id);
                    feeModel.Status = request.Status;
                    feeModel.UpdateDate = DateTime.UtcNow;
                    _professionalRepository.Update(feeModel);
                }
                if (Commit())
                {
                    _bus.RaiseEvent(new DomainNotification(request.MessageType, null, request.Professionals));
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

        public Task<bool> Handle(DeleteProfessionalsCommand request, CancellationToken cancellationToken)
        {
            try
            {
                foreach (var feeId in request.Ids)
                {
                    var feeModel = _professionalRepository.GetById(feeId);
                    feeModel.Status = eRecordStatus.Deleted;
                    feeModel.DeletedAt = DateTime.UtcNow;
                    //proModel.DeletedById = 0;
                    _professionalRepository.Update(feeModel);
                }
                if (Commit())
                {
                    _bus.RaiseEvent(new DomainNotification(request.MessageType, null, request.Ids));
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