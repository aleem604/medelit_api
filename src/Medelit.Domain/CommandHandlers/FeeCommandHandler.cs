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
    public class FeeCommandHandler : CommandHandler,
        IRequestHandler<SaveFeeCommand, bool>,
        IRequestHandler<UpdateFeesStatusCommand, bool>,
        IRequestHandler<DeleteFeesCommand, bool>
    {
        private readonly IMapper _mapper;
        private readonly IMediatorHandler _bus;
        private readonly IFeeRepository _feeRepository;
        private readonly IConfiguration _config;


        public FeeCommandHandler(IMapper mapper,
            IUnitOfWork unitOfWork,
            IConfiguration config,
            IMediatorHandler bus,
            IHttpContextAccessor httpContext,
            IFeeRepository feeRepository,

            INotificationHandler<DomainNotification> notifications)
            : base(bus, notifications, httpContext, unitOfWork)
        {
            _mapper = mapper;
            _config = config;
            _bus = bus;
            _feeRepository = feeRepository;
        }

        public Task<bool> Handle(SaveFeeCommand request, CancellationToken cancellationToken)
        {
            try
            {
                bool commmitResult;
                if (request.Fee.Id > 0)
                {
                    var feeModel = _feeRepository.GetById(request.Fee.Id);
                    feeModel.FeeName = request.Fee.FeeName;
                    feeModel.FeeTypeId = request.Fee.FeeTypeId;
                    feeModel.A1 = request.Fee.A1;
                    feeModel.A2 = request.Fee.A2;
                    _feeRepository.Update(feeModel);
                    commmitResult = Commit();
                    request.Fee = feeModel;

                    //var allFees = _feeRepository.GetAll();
                    //foreach (var fee in allFees)
                    //{
                       
                    //        fee.FeeCode = fee.FeeTypeId == eFeeType.PTFee ? $"FP{fee.Id.ToString().PadLeft(6, '0')}" : $"FS{fee.Id.ToString().PadLeft(6, '0')}";
                    //        fee.UpdateDate = DateTime.UtcNow;
                    //        _feeRepository.Update(fee);
                        
                    //}
                    //Commit();
                }
                else
                {
                    var feeModel = request.Fee;
                    feeModel.CreateDate = DateTime.UtcNow;
                    _feeRepository.Add(feeModel);
                    commmitResult = Commit();
                    if (commmitResult && feeModel.Id > 0)
                    {
                        var id = feeModel.Id;
                        feeModel.FeeCode = feeModel.FeeTypeId == eFeeType.PTFee ? $"FP{id.ToString().PadLeft(6, '0')}" : $"FS{id.ToString().PadLeft(6, '0')}";
                        _feeRepository.Update(feeModel);
                        commmitResult = Commit();
                    }
                    request.Fee = feeModel;
                }
                if (commmitResult)
                {
                    _bus.RaiseEvent(new DomainNotification(request.MessageType, null, request.Fee));
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
        public Task<bool> Handle(UpdateFeesStatusCommand request, CancellationToken cancellationToken)
        {
            try
            {
                foreach (var fee in request.Fees)
                {
                    var feeModel = _feeRepository.GetById(fee.Id);
                    feeModel.Status = request.Status;
                    feeModel.UpdateDate = DateTime.UtcNow;
                    _feeRepository.Update(feeModel);
                }
                if (Commit())
                {
                    _bus.RaiseEvent(new DomainNotification(request.MessageType, null, request.Fees));
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

        public Task<bool> Handle(DeleteFeesCommand request, CancellationToken cancellationToken)
        {
            try
            {
                foreach (var feeId in request.FeeIds)
                {
                    var feeModel = _feeRepository.GetById(feeId);
                    feeModel.Status = eRecordStatus.Deleted;
                    feeModel.DeletedAt = DateTime.UtcNow;
                    //feeModel.DeletedById = 0;
                    _feeRepository.Update(feeModel);
                }
                if (Commit())
                {
                    _bus.RaiseEvent(new DomainNotification(request.MessageType, null, request.FeeIds));
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