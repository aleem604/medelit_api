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

namespace Medelit.Application
{
    public class ServiceService : IServiceService
    {
        private readonly IServiceRepository _serviceRepository;
        private readonly ITitleRepository _titleRepository;
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
                            ILanguageRepository langRepository,
                            ITitleRepository titleRepository
            
            )
        {
            _mapper = mapper;
            _httpContext = httpContext;
            _configuration = configuration;
            _bus = bus;
            _serviceRepository = serviceRepository;
            _titleRepository = titleRepository;
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
            viewModel.Professionals = service.ServiceProfessionalRelation.Select((s) => new FilterModel { Id = s.ProfessionalId }).ToList();
            return viewModel;

        }

        public dynamic FindServices(SearchViewModel viewModel)
        {
            viewModel.Filter = viewModel.Filter ?? new SearchFilterViewModel();
            var query = _serviceRepository.GetAll().Where(x=>x.Status != eRecordStatus.Deleted);

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


        public void SaveService(ServiceViewModel viewModel)
        {
            var serviceModel = _mapper.Map<Service>(viewModel);
            serviceModel.ServiceProfessionalRelation = viewModel.Professionals.Select((s) => new ServiceProfessionalRelation { ProfessionalId = s.Id }).ToList();
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



        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }

    }
}