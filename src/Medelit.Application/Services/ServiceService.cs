﻿using System;
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

        public dynamic FindServices(SearchViewModel viewModel)
        {
            viewModel.Filter = viewModel.Filter ?? new SearchFilterViewModel();
            var fees = _feeRepository.GetAll().ToList();
            var professionals = _professionalRepository.GetAll().Select(x => new FilterModel { Id = x.Id, Value = x.Name }).ToList();

            var query = _serviceRepository.GetAllWithProfessionals().Where(x => x.Status != eRecordStatus.Deleted)
                .Select((s) => new {
                    s.Id,
                    s.Name,
                    //PTFee = GetString(fees.FirstOrDefault(x=>x.Id == s.PTFeeId)?.A1, fees.FirstOrDefault(x=>x.Id == s.PTFeeId)?.A2),
                    //ProFee = GetString(fees.FirstOrDefault(x => x.Id == s.PROFeeId)?.A1, fees.FirstOrDefault(x => x.Id == s.PROFeeId)?.A2),
                    //Professionals = PopulateProfessionals(s.ServiceProfessionals, professionals),
                    s.Covermap,
                    s.Status,
                    s.CreateDate,
                    s.CreatedById,
                });

            if (!string.IsNullOrEmpty(viewModel.Filter.Search))
            {
                viewModel.Filter.Search = viewModel.Filter.Search.Trim();
                query = query.Where(x =>
                (
                    (!string.IsNullOrEmpty(x.Name) && x.Name.CLower().Contains(viewModel.Filter.Search.CLower()))
                || (x.Name.Equals(viewModel.Filter.Search))
                || (x.Id.ToString().Contains(viewModel.Filter.Search))

                ));
            }

            if (viewModel.Filter.Status != eRecordStatus.All)
            {
                query = query.Where(x => x.Status == viewModel.Filter.Status);
            }

            switch (viewModel.SortField)
            {
                case "name":
                    if (viewModel.SortOrder.Equals("asc"))
                        query = query.OrderBy(x => x.Name);
                    else
                        query = query.OrderByDescending(x => x.Name);
                    break;

                case "professional":
                    if (viewModel.SortOrder.Equals("asc"))
                        query = query.OrderBy(x => x.Name);
                    else
                        query = query.OrderByDescending(x => x.Name);
                    break;

                case "status":
                    if (viewModel.SortOrder.Equals("asc"))
                        query = query.OrderBy(x => x.Status);
                    else
                        query = query.OrderByDescending(x => x.Status);
                    break;
                case "createDate":
                    if (viewModel.SortOrder.Equals("asc"))
                        query = query.OrderBy(x => x.CreateDate);
                    else
                        query = query.OrderByDescending(x => x.CreateDate);
                    break;
                case "createdBy":
                    if (viewModel.SortOrder.Equals("asc"))
                        query = query.OrderBy(x => x.CreatedById);
                    else
                        query = query.OrderByDescending(x => x.CreatedById);
                    break;

                default:
                    if (viewModel.SortOrder.Equals("asc"))
                        query = query.OrderBy(x => x.Id);
                    else
                        query = query.OrderByDescending(x => x.Id);

                    break;
            }

            var totalCount = query.LongCount();

            return new
            {
                items = query.Skip(viewModel.PageNumber * viewModel.PageSize).Take(viewModel.PageSize).ToList(),
                totalCount
            };
        }

        private string GetString(decimal? a1, decimal? a2)
        {
            var sb = new StringBuilder();
            if (a1.HasValue)
                sb.Append(a1.Value.ToString("G29"));
            if(a2.HasValue)
            {
                if (a1.HasValue)
                    sb.Append(", ");
                sb.Append(a2.Value.ToString("G29"));
            }
            return sb.ToString();
        }

        private string PopulateProfessionals(IEnumerable<ServiceProfessionalPtFees> services, List<FilterModel> professionals)
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

        public void AddUpdateFeeForService(AddUpdateFeeToServiceViewModel viewModel) {
            var model = _mapper.Map<AddUpdateFeeToService>(viewModel);

            _serviceRepository.AddUpdateFeeForService(model);
        }



        public dynamic GetProfessionalRelations(long proId)
        {
            return _serviceRepository.GetProfessionalServicesWithInclude(proId);
        }
        public dynamic GetProfessionalFeesDetail(long serviceId)
        {
            return _serviceRepository.GetProfessionalFeesDetail(serviceId);
        }

        public dynamic GetServiceConnectedProfessionals(long serviceId)
        {
            return _serviceRepository.GetServiceConnectedProfessionals(serviceId);
        }
         public dynamic GetConnectedCustomersInvoicingEntities(long serviceId)
        {
            return _serviceRepository.GetConnectedCustomersInvoicingEntities(serviceId);
        }

        public dynamic GetConnectedBookings(long serviceId) {
            return _serviceRepository.GetConnectedBookings(serviceId);
        }
        public dynamic GetConnectedCustomerInvoices(long serviceId) {
            return _serviceRepository.GetConnectedCustomerInvoices(serviceId);
        }
        public dynamic GetConnectedLeads(long serviceId) {
            return _serviceRepository.GetConnectedLeads(serviceId);
        }

        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }

    }
}