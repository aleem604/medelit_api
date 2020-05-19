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

        private string PopulateProfessionals(IEnumerable<ServiceProfessionalFees> services, List<FilterModel> professionals)
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

        public void GetServiceTags()
        {
            var retTags = new List<string>();
            var tags = _serviceRepository.GetAll().Select(s => s.Tags).ToList();
            tags.ForEach(t => { 
            if(!string.IsNullOrEmpty(t))
                {
                    var tagArr = t.Split(',');
                    retTags.AddRange(tagArr);
                }
            });
            _bus.RaiseEvent(new DomainNotification(GetType().Name, null, retTags));

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

        public void SaveProfessionalsWithFeesToConnectWithService(IEnumerable<long> proIds, long serviceId)
        {
            _serviceRepository.SaveProfessionalsWithFeesToConnectWithService(proIds, serviceId);
        }
        public void RemoveProfessionalsFromService(IEnumerable<long> model, long serviceId)
        {
            _serviceRepository.RemoveProfessionalsFromService(model, serviceId);
        }

        public void GetServiceProfessionalFeeRowDetail(long rowId)
        {
            _serviceRepository.GetServiceProfessionalFeeRowDetail(rowId);
        }
        public void GetServiceProfessionalFeesForFilter(long rowId)
        {
            _serviceRepository.GetServiceProfessionalFeesForFilter(rowId);
        }

         public void SaveProfessionalServicesFees(ProfessionalConnectedServicesModel model, long rowId)
        {
            _serviceRepository.SaveProfessionalServicesFees(model, rowId);
        }



        #region service connect fees
        public void GetServiceConnectedFees(long serviceId, eFeeType feeType)
        {
            _serviceRepository.GetServiceConnectedFees(serviceId, feeType);
        }
        public void GetServiceConnectedFeesToConnect(long serviceId, eFeeType feeType)
        {
            _serviceRepository.GetServiceConnectedFeesToConnect(serviceId, feeType);
        }
        public void SaveFeesForService(IEnumerable<long> model, long serviceId, eFeeType feeType)
        {
            _serviceRepository.SaveFeesForService(model, serviceId, feeType);
        }
        public void DetachFeeFromService(IEnumerable<long> model, long serviceId, eFeeType feeType)
        {
            _serviceRepository.DetachFeeFromService(model, serviceId, feeType);
        }
        #endregion service connect pt fees

        public dynamic GetServiceProfessionals(ServicFilterViewModel viewModel)
        {
            return _serviceRepository.GetServiceProfessionals(viewModel.ProfessionalId, viewModel.FieldId, viewModel.SubCategoryId, viewModel.Tag);
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

            //_serviceRepository.AddUpdateFeeForService(model);
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