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
                if (request.Fee.Id > 0)
                {
                    if (request.Fee.FeeTypeId == eFeeType.PTFee)
                    {
                        var ptFeeModel = _feeRepository.GetPtFee(request.Fee.Id);
                        ptFeeModel.FeeName = request.Fee.FeeName;
                        ptFeeModel.A1 = request.Fee.A1;
                        ptFeeModel.A2 = request.Fee.A2;
                        ptFeeModel.Tags = request.Fee.Tags;
                        ptFeeModel.UpdateDate = request.Fee.UpdateDate;
                        ptFeeModel.UpdatedById = CurrentUser.Id;
                        if (string.IsNullOrEmpty(ptFeeModel.AssignedToId))
                            ptFeeModel.AssignedToId = CurrentUser.Id;

                        _feeRepository.UpdatePtFee(ptFeeModel);
                        _bus.RaiseEvent(new DomainNotification(GetType().Name, null, ptFeeModel));
                        return Task.FromResult(true);
                    }
                    else
                    {
                        var proFeeModel = _feeRepository.GetProFee(request.Fee.Id);
                        proFeeModel.FeeName = request.Fee.FeeName;
                        proFeeModel.A1 = request.Fee.A1;
                        proFeeModel.A2 = request.Fee.A2;
                        proFeeModel.Tags = request.Fee.Tags;
                        proFeeModel.UpdateDate = request.Fee.UpdateDate;
                        proFeeModel.UpdatedById = CurrentUser.Id;
                        if (string.IsNullOrEmpty(proFeeModel.AssignedToId))
                            proFeeModel.AssignedToId = CurrentUser.Id;

                        _feeRepository.UpdateProFee(proFeeModel);
                        _bus.RaiseEvent(new DomainNotification(GetType().Name, null, proFeeModel));
                        return Task.FromResult(true);
                    }
                }
                else
                {
                    if (request.Fee.FeeTypeId == eFeeType.PTFee)
                    {
                        var ptFee = new PtFee
                        {
                            FeeName = request.Fee.FeeName,
                            A1 = request.Fee.A1,
                            A2 = request.Fee.A2,
                            Tags = request.Fee.Tags,
                            CreateDate = request.Fee.CreateDate,
                            CreatedById = CurrentUser.Id,
                            AssignedToId = CurrentUser.Id
                        };
                        _feeRepository.AddPtFee(ptFee);
                        if (ptFee.Id > 0)
                        {
                            var updatedFee = _feeRepository.GetPtFee(ptFee.Id);
                            updatedFee.FeeCode = $"FP{ptFee.Id.ToString().PadLeft(6, '0')}";
                            _feeRepository.UpdatePtFee(updatedFee);
                            _bus.RaiseEvent(new DomainNotification(GetType().Name, null, updatedFee));
                            return Task.FromResult(true);
                        }
                    }
                    else
                    {
                        var proFee = new ProFee
                        {
                            FeeName = request.Fee.FeeName,
                            A1 = request.Fee.A1,
                            A2 = request.Fee.A2,
                            Tags = request.Fee.Tags,
                            CreateDate = request.Fee.CreateDate,
                            CreatedById = CurrentUser.Id,
                            AssignedToId = CurrentUser.Id
                        };
                        _feeRepository.AddProFee(proFee);
                        if (proFee.Id > 0)
                        {
                            var updatedFee = _feeRepository.GetProFee(proFee.Id);
                            updatedFee.FeeCode = $"FS{proFee.Id.ToString().PadLeft(6, '0')}";
                            _feeRepository.UpdateProFee(updatedFee);
                            _bus.RaiseEvent(new DomainNotification(GetType().Name, null, updatedFee));
                            return Task.FromResult(true);
                        }
                    }
                }
                _bus.RaiseEvent(new DomainNotification(request.MessageType, MessageCodes.ERROR_OCCURED));
                return Task.FromResult(false);
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
                    if (fee.FeeTypeId == eFeeType.PTFee)
                    {
                        var feeModel = _feeRepository.GetPtFee(fee.Id);
                        feeModel.Status = request.Status;
                        feeModel.UpdateDate = DateTime.UtcNow;
                        feeModel.UpdatedById = CurrentUser.Id;
                        _feeRepository.UpdatePtFee(feeModel);
                    }
                    else
                    {
                        var feeModel = _feeRepository.GetProFee(fee.Id);
                        feeModel.Status = request.Status;
                        feeModel.UpdateDate = DateTime.UtcNow;
                        feeModel.UpdatedById = CurrentUser.Id;
                        _feeRepository.UpdateProFee(feeModel);
                    }
                }
                _bus.RaiseEvent(new DomainNotification(request.MessageType, null, request.Fees));
                return Task.FromResult(true);
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
                foreach (var fee in request.VFees)
                {
                    if (fee.FeeTypeId == eFeeType.PTFee)
                    {
                        var feeModel = _feeRepository.GetPtFee(fee.Id);
                        _feeRepository.RemovePtFee(feeModel);
                    }
                    else
                    {
                        var feeModel = _feeRepository.GetProFee(fee.Id);
                        _feeRepository.RemoveProFee(feeModel);
                    }
                }

                _bus.RaiseEvent(new DomainNotification(request.MessageType, null, request.VFees));
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