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
using System.Text;
using Medelit.Common.Models;
using Medelit.Domain.Core.Notifications;

namespace Medelit.Application
{
    public class ServiceService : IServiceService
    {
        private readonly IServiceRepository _serviceRepository;
        private readonly IProfessionalRepository _professionalRepository;
        private readonly IFeeRepository _feeRepository;
        private readonly ILanguageRepository _langRepository;

        private readonly IMapper _mapper;
        private readonly IConfiguration _configuration;
        private readonly IHttpContextAccessor _httpContext;
        private readonly IMediatorHandler _bus;

        public ServiceService(IMapper mapper,
                            IHttpContextAccessor httpContext,
                            IConfiguration configuration,
                            IMediatorHandler bus,
                            IServiceRepository serviceRepository,
                            IProfessionalRepository professionalRepository,
                            IFeeRepository feeRepository,
                            ILanguageRepository langRepository
            )
        {
            _mapper = mapper;
            _httpContext = httpContext;
            _configuration = configuration;
            _bus = bus;
            _serviceRepository = serviceRepository;
            _professionalRepository = professionalRepository;
            _feeRepository = feeRepository;
            _langRepository = langRepository;
        }

        public dynamic GetServices()
        {
            return _serviceRepository.GetAll().ToList();
        }

        public ServiceViewModel GetServiceById(long serviceId)
        {
            //return _mapper.Map<ServiceViewModel>(_serviceRepository.GetByIdWithIncludes(serviceId));

            var service = _serviceRepository.GetByIdWithIncludes(serviceId);
            var viewModel = _mapper.Map<ServiceViewModel>(service);
            //viewModel.Professionals = service.ServiceProfessionals.Select((s) => new FilterModel { Id = s.ProfessionalId, Value = s.Professional?.Name }).ToList();
            return viewModel;

        }

        public void FindServices(SearchViewModel viewModel)
        {
            _serviceRepository.FindServices(viewModel);
        }

        private string GetString(decimal? a1, decimal? a2)
        {
            var sb = new StringBuilder();
            if (a1.HasValue)
                sb.Append(a1.Value.ToString("G29"));
            if (a2.HasValue)
            {
                if (a1.HasValue)
                    sb.Append(", ");
                sb.Append(a2.Value.ToString("G29"));
            }
            return sb.ToString();
        }

        private string PopulateProfessionals(IEnumerable<ServiceProfessionals> services, List<FilterModel> professionals)
        {
            var query = from s in services
                        join
                        p in professionals on s.ProfessionalId equals p.Id
                        select p.Value;

            return string.Join(",", query.ToArray());
        }


        public void SaveService(ServiceViewModel viewModel)
        {
            var serviceModel = _mapper.Map<Service>(viewModel);
            //serviceModel.ServiceProfessionals = viewModel.Professionals.Select((s) => new ServiceProfessionalRelation { ProfessionalId = s.Id }).ToList();
            _bus.SendCommand(new SaveServiceCommand { Service = serviceModel });
        }

        public void UpdateStatus(IEnumerable<ServiceViewModel> services, eRecordStatus status)
        {
            _bus.SendCommand(new UpdateServicesStatusCommand { Services = _mapper.Map<IEnumerable<Service>>(services), Status = status });
        }

        public void DeleteServices(IEnumerable<long> serviceIds)
        {
            _bus.SendCommand(new DeleteServicesCommand { ServiceIds = serviceIds });
        }

        public void GetServiceConnectedProfessionals(long serviceId)
        {
            _serviceRepository.GetServiceConnectedProfessionals(serviceId);
        }

        public void GetProfessionalsWithFeesToConnectWithService(long serviceId)
        {
            _serviceRepository.GetProfessionalsWithFeesToConnectWithService(serviceId);
        }

        public void SaveProfessionalsWithFeesToConnectWithService(IEnumerable<EditProfessionalServiceFeesModel> model, long serviceId)
        {
            _serviceRepository.SaveProfessionalsWithFeesToConnectWithService(model, serviceId);
        }
        public void RemoveProfessionalsFromService(IEnumerable<EditProfessionalServiceFeesModel> model, long serviceId)
        {
            _serviceRepository.RemoveProfessionalsFromService(model, serviceId);
        }

        #region service connect pt fees
        public void GetServiceConnectedPtFees(long serviceId)
        {
            _serviceRepository.GetServiceConnectedPtFees(serviceId);
        }
        public void GetServiceConnectedPtFeesToConnect(long serviceId)
        {
            _serviceRepository.GetServiceConnectedPtFeesToConnect(serviceId);
        }
        public void SavePtFeesForService(IEnumerable<ServiceConnectedPtFeesModel> model, long serviceId)
        {
            _serviceRepository.SavePtFeesForService(model, serviceId);
        }
        public void DetachPtFeeFromService(IEnumerable<ServiceConnectedPtFeesModel> model, long serviceId)
        {
            _serviceRepository.DetachPtFeeFromService(model, serviceId);
        }
        #endregion service connect pt fees

        #region service connect pro fees
        public void GetServiceConnectedProFees(long serviceId)
        {
            _serviceRepository.GetServiceConnectedProFees(serviceId);
        }
        public void GetServiceConnectedProFeesToConnect(long serviceId)
        {
            _serviceRepository.GetServiceConnectedProFeesToConnect(serviceId);
        }
        public void SaveProFeesForService(IEnumerable<ServiceConnectedProFeesModel> model, long serviceId)
        {
            _serviceRepository.SaveProFeesForService(model, serviceId);
        }
        public void DetachProFeeFromService(IEnumerable<ServiceConnectedProFeesModel> model, long serviceId)
        {
            _serviceRepository.SaveProFeesForService(model, serviceId);
        }
        #endregion service connect pt fees






        public dynamic GetProfessionalServices(ServicFilterViewModel viewModel)
        {
            return _serviceRepository.GetProfessionalServices(viewModel.ProfessionalId, viewModel.FieldId, viewModel.SubCategoryId, viewModel.Tag);
        }

        public void SaveProfessionalServices(IEnumerable<long> serviceIds, long proId)
        {
            _bus.SendCommand(new AddProfessionalToServicesCommand { ServiceIds = serviceIds, ProfessionalId = proId });
        }

        public void DetachProfessional(long serviceId, long proId)
        {
            _bus.SendCommand(new DetachProfessionalCommand { ServiceId = serviceId, ProfessionalId = proId });
        }

        public void AddUpdateFeeForService(AddUpdateFeeToServiceViewModel viewModel)
        {
            var model = _mapper.Map<AddUpdateFeeToService>(viewModel);

            _serviceRepository.AddUpdateFeeForService(model);
        }



        public dynamic GetProfessionalRelations(long proId)
        {
            return _serviceRepository.GetProfessionalServicesWithInclude(proId);
        }

        public dynamic GetConnectedCustomersInvoicingEntities(long serviceId)
        {
            return _serviceRepository.GetConnectedCustomersInvoicingEntities(serviceId);
        }

        public dynamic GetConnectedBookings(long serviceId)
        {
            return _serviceRepository.GetConnectedBookings(serviceId);
        }
        public dynamic GetConnectedCustomerInvoices(long serviceId)
        {
            return _serviceRepository.GetConnectedCustomerInvoices(serviceId);
        }
        public dynamic GetConnectedLeads(long serviceId)
        {
            return _serviceRepository.GetConnectedLeads(serviceId);
        }

        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }

    }
}