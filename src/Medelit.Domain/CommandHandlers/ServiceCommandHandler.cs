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
using System.Linq;

namespace Medelit.Domain.CommandHandlers
{
    public class ServiceCommandHandler : CommandHandler,
        IRequestHandler<SaveServiceCommand, bool>,
        IRequestHandler<UpdateServicesStatusCommand, bool>,
        IRequestHandler<DeleteServicesCommand, bool>,
        IRequestHandler<AddProfessionalToServicesCommand, bool>,
        IRequestHandler<DetachProfessionalCommand, bool>
    {
        private readonly IMapper _mapper;
        private readonly IMediatorHandler _bus;
        private readonly IServiceRepository _serviceRepository;
        private readonly IFieldSubcategoryRepository _fieldSubcategoryRepository;
        private readonly IConfiguration _config;


        public ServiceCommandHandler(IMapper mapper,
            IUnitOfWork unitOfWork,
            IConfiguration config,
            IMediatorHandler bus,
            IHttpContextAccessor httpContext,
            IServiceRepository serviceRepository,
            IFieldSubcategoryRepository fieldSubcategoryRepository,

            INotificationHandler<DomainNotification> notifications)
            : base(bus, notifications, httpContext, unitOfWork)
        {
            _mapper = mapper;
            _config = config;
            _bus = bus;
            _serviceRepository = serviceRepository;
            _fieldSubcategoryRepository = fieldSubcategoryRepository;
        }

        public Task<bool> Handle(SaveServiceCommand request, CancellationToken cancellationToken)
        {
            try
            {
                bool commmitResult;
                if (request.Service.Id > 0)
                {
                    var vm = request.Service;
                    var serviceModel = _serviceRepository.GetById(request.Service.Id);
                    serviceModel.UpdateDate = DateTime.UtcNow;

                    serviceModel.Name = vm.Name;
                    serviceModel.CycleId = vm.CycleId;
                    serviceModel.ActiveServiceId = vm.ActiveServiceId;
                    serviceModel.TimedServiceId = vm.TimedServiceId;
                    serviceModel.ContractedServiceId = vm.ContractedServiceId;
                    serviceModel.InformedConsentId = vm.InformedConsentId;
                    serviceModel.FieldId = vm.FieldId;
                    serviceModel.SubcategoryId = vm.SubcategoryId;
                    serviceModel.DurationId = vm.DurationId;
                    serviceModel.VatId = vm.VatId;
                    serviceModel.Covermap = vm.Covermap;
                    serviceModel.PTFeeId = vm.PTFeeId;
                    serviceModel.PROFeeId = vm.PROFeeId;
                    serviceModel.Tags = vm.Tags;
                    serviceModel.InvoicingNotes = vm.InvoicingNotes;
                    serviceModel.RefundNotes = vm.RefundNotes;
                    var fieldCode = _fieldSubcategoryRepository.GetAll().Where(x => x.Id == serviceModel.FieldId).FirstOrDefault().Code;
                    serviceModel.ServiceCode = $"{fieldCode}{vm.Id.ToString().PadLeft(6, '0')}";
                    _serviceRepository.RemoveProfessionals(request.Service.Id);

                    serviceModel.ServiceProfessionals = vm.ServiceProfessionals;

                    serviceModel.UpdateDate = DateTime.UtcNow;
                    serviceModel.UpdatedById = CurrentUser.Id;

                    _serviceRepository.Update(serviceModel);
                    commmitResult = Commit();
                    request.Service = serviceModel;

                    //var allServices = _serviceRepository.GetAll();
                    //foreach (var service in allServices)
                    //{

                    //        service.ServiceCode = service.ServiceTypeId == eServiceType.PTService ? $"FP{service.Id.ToString().PadLeft(6, '0')}" : $"FS{service.Id.ToString().PadLeft(6, '0')}";
                    //        service.UpdateDate = DateTime.UtcNow;
                    //        _serviceRepository.Update(service);

                    //}
                    //Commit();
                }
                else
                {
                    var serviceModel = request.Service;
                    serviceModel.CreateDate = DateTime.UtcNow;
                    serviceModel.Status = eRecordStatus.Pending;

                    serviceModel.CreateDate = DateTime.UtcNow;
                    serviceModel.CreatedById = CurrentUser.Id;

                    _serviceRepository.Add(serviceModel);
                    commmitResult = Commit();
                    if (commmitResult && serviceModel.Id > 0)
                    {
                        var id = serviceModel.Id;
                        var fieldCode = _fieldSubcategoryRepository.GetAll().Where(x => x.Id == serviceModel.FieldId).FirstOrDefault().Code;
                        serviceModel.ServiceCode = $"{fieldCode}{id.ToString().PadLeft(6, '0')}";

                        serviceModel.UpdateDate = DateTime.UtcNow;
                        serviceModel.UpdatedById = CurrentUser.Id;

                        _serviceRepository.Update(serviceModel);
                        commmitResult = Commit();
                    }
                    request.Service = serviceModel;
                }
                if (commmitResult)
                {
                    _bus.RaiseEvent(new DomainNotification(request.MessageType, null, request.Service));
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
        public Task<bool> Handle(UpdateServicesStatusCommand request, CancellationToken cancellationToken)
        {
            try
            {
                foreach (var service in request.Services)
                {
                    var serviceModel = _serviceRepository.GetById(service.Id);
                    serviceModel.Status = request.Status;
                    serviceModel.UpdateDate = DateTime.UtcNow;
                    service.UpdatedById = CurrentUser.Id;

                    _serviceRepository.Update(serviceModel);
                }
                if (Commit())
                {
                    _bus.RaiseEvent(new DomainNotification(request.MessageType, null, request.Services));
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

        public Task<bool> Handle(DeleteServicesCommand request, CancellationToken cancellationToken)
        {
            try
            {
                foreach (var serviceId in request.ServiceIds)
                {
                    var serviceModel = _serviceRepository.GetById(serviceId);
                    serviceModel.Status = eRecordStatus.Deleted;
                    serviceModel.DeletedAt = DateTime.UtcNow;
                    serviceModel.DeletedById = CurrentUser.Id;
                    _serviceRepository.Update(serviceModel);
                }
                if (Commit())
                {
                    _bus.RaiseEvent(new DomainNotification(request.MessageType, null, request.ServiceIds));
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

        public Task<bool> Handle(AddProfessionalToServicesCommand request, CancellationToken cancellationToken)
        {
            try
            {
                foreach (var serviceId in request.ServiceIds)
                {
                    _serviceRepository.AddProfessionalToService(serviceId, request.ProfessionalId);

                }
                if (Commit())
                {
                    _bus.RaiseEvent(new DomainNotification(request.MessageType, null, request.ServiceIds));
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

        public Task<bool> Handle(DetachProfessionalCommand request, CancellationToken cancellationToken)
        {
            try
            {
                _serviceRepository.DetachProfessional(request.ServiceId, request.ProfessionalId);
                

                if (Commit())
                {
                    var relations = _serviceRepository.GetProfessionalServicesWithInclude(request.ProfessionalId);
                    _bus.RaiseEvent(new DomainNotification(request.MessageType, null, relations));
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
        {

        }

    }
}