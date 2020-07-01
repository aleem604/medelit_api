using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Medelit.Common;
using Medelit.Domain.Commands;
using Medelit.Domain.Core.Bus;
using Medelit.Domain.Interfaces;
using System.Linq;
using System.Collections.Generic;
using Medelit.Domain.Models;
using Medelit.Infra.CrossCutting.Identity.Data;
using Medelit.Common.Models;
using Medelit.Domain.Core.Notifications;
using Microsoft.AspNetCore.Hosting;

namespace Medelit.Application
{
    public class ProfessionalService : BaseService, IProfessionalService
    {
        private readonly IProfessionalRepository _professionalRepository;
        private readonly ILanguageRepository _langRepository;
        private readonly IFieldSubcategoryRepository _fieldRepository;
        private readonly IServiceRepository _serviceRepository;
        private readonly IStaticDataRepository _dataRepository;
        private readonly IMapper _mapper;
        private readonly IConfiguration _configuration;
        private readonly IHttpContextAccessor _httpContext;
        private readonly IMediatorHandler _bus;

        public ProfessionalService(IMapper mapper,
            ApplicationDbContext context,
                            IHttpContextAccessor httpContext,
                            IConfiguration configuration,
                            IMediatorHandler bus,
                            IProfessionalRepository professionalRepository,
                            ILanguageRepository langRepository,
                            IFieldSubcategoryRepository fieldRepository,
                            IServiceRepository serviceRepository,
                            IStaticDataRepository dataRepository,
                            IHostingEnvironment env

            ) : base(context, httpContext, configuration, env)
        {
            _mapper = mapper;
            _bus = bus;
            _professionalRepository = professionalRepository;
            _langRepository = langRepository;
            _fieldRepository = fieldRepository;
            _serviceRepository = serviceRepository;
            _dataRepository = dataRepository;
        }

        public dynamic GetProfessionals()
        {
            return _professionalRepository.GetAll().Count();
        }

        public void FindProfessionals(SearchViewModel viewModel)
        {
            _professionalRepository.FindProfessionals(viewModel);
        }

        public void GetProfessionalById(long professionalId)
        {
            try
            {
                var professional = _professionalRepository.GetByIdWithIncludes(professionalId).FirstOrDefault();
                //var professionalServices = _professionalRepository.ServiceProfessionalFees(professional.Id);

                var viewModel = _mapper.Map<ProfessionalViewModel>(professional);
                viewModel.AssignedTo = GetAssignedUser(viewModel.AssignedToId);
                //viewModel.ProfessionalServices = _mapper.Map<IEnumerable<ServiceProfessionalRelationVeiwModel>>(professionalServices);

                _bus.RaiseEvent(new DomainNotification(GetType().Name, null, viewModel));
            }
            catch (Exception ex)
            {
                _bus.RaiseEvent(new DomainNotification(GetType().Name, ex.Message));
            }
         
        }

        public void SaveProvessional(ProfessionalViewModel model)
        {
            var professionalModel = _mapper.Map<Professional>(model);
            _bus.SendCommand(new SaveProfessionalCommand { Model = professionalModel });

        }
        public void UpdateStatus(IEnumerable<ProfessionalViewModel> pros, eRecordStatus status)
        {
            _bus.SendCommand(new UpdateProfessionalsStatusCommand { Professionals = _mapper.Map<IEnumerable<Professional>>(pros), Status = status });
        }

        public void DeleteFees(IEnumerable<long> feeIds)
        {
            _bus.SendCommand(new DeleteProfessionalsCommand { Ids = feeIds });
        }

        public dynamic GetConnectedCustomers(long proId)
        {
            return _professionalRepository.GetConnectedCustomers(proId);
        }

        public dynamic GetConnectedBookings(long proId)
        {
            return _professionalRepository.GetConnectedBookings(proId);
        }

        public dynamic GetConnectedInvoices(long proId)
        {
            return _professionalRepository.GetConnectedInvoices(proId);
        }
        public dynamic GetConnectedLeads(long proId)
        {
            return _professionalRepository.GetConnectedLeads(proId);
        }

        public void GetProfessionalConnectedServices(long proId)
        {
            _professionalRepository.GetProfessionalConnectedServices(proId);
        }

        public void DetachProfessionalConnectedService(IEnumerable<EditProfessionalServiceFeesModel> serviceIds, long proId)
        {
            _professionalRepository.DetachProfessionalConnectedService(serviceIds, proId);
        }

        public dynamic GetProfessionalServiceDetail(long professionalPtFeeRowId, long professionalProFeeRowId)
        {
            return _professionalRepository.GetProfessionalServiceDetail(professionalPtFeeRowId, professionalProFeeRowId);
        }

        public void SaveProfessionalServiceDetail(FullFeeViewModel model)
        {
            var domainModel = _mapper.Map<EditProfessionalServiceFeesModel>(model);
            _professionalRepository.SaveProfessionalServiceDetail(domainModel);
        }

        public void GetServicesToAttachWithProfessional(long proId)
        {
            _professionalRepository.GetServicesToAttachWithProfessional(proId);
        }

        public void GetServicesForConnectFilter(long proId)
        {
            _professionalRepository.GetServicesForConnectFilter(proId);
        }

        public void AttachServicesToProfessional(IEnumerable<long> serviceIds, long proId)
        {
            _professionalRepository.AttachServicesToProfessional(serviceIds, proId);
        }

        public void GetFeesForFilterToConnectWithServiceProfessional(long serviceId, long professionalId)
        {
            _professionalRepository.GetFeesForFilterToConnectWithServiceProfessional(serviceId, professionalId);
        }

        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }

    }
}