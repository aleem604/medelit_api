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
                            IStaticDataRepository dataRepository

            ) : base(context)
        {
            _mapper = mapper;
            _httpContext = httpContext;
            _configuration = configuration;
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

        public dynamic FindProfessionals(SearchViewModel viewModel)
        {
            viewModel.Filter = viewModel.Filter ?? new SearchFilterViewModel();
            var langs = _langRepository.GetAll().ToList();
            var fields = _fieldRepository.GetAll().ToList();
            var services = _serviceRepository.GetAll().ToList();
            var serviceProfessionals = _serviceRepository.GetServiceProfessionals().ToList();
            var cities = _dataRepository.GetCities().ToList();

            var query = _professionalRepository.GetAll().Select((s) => new
            {
                s.Id,
                s.Name,
                s.Telephone,
                s.Email,
                s.CoverMap,
                Field = string.Join("<br/>", s.ProfessionalFields.Select(x=>x.Field.Field).Distinct().ToList()),
                SubCategory = string.Join("<br/>", s.ProfessionalSubCategories.Select(x => x.SubCategory.SubCategory).Distinct().ToList()),
                Services = GetServices(s.Id, services, serviceProfessionals),
                City = s.CityId > 0 ? cities.FirstOrDefault(c => c.Id == s.CityId).Value : "",
                s.ContractDate,
                s.ContractEndDate,

                s.ContractStatusId
            });

            if (viewModel.Filter.ProfessionalFilter == eProfessionalFilter.CurrentProfessionals)
            {
                query = query.Where(x => x.ContractStatusId == (short)eContractStatus.Active);
            }
            else if (viewModel.Filter.ProfessionalFilter == eProfessionalFilter.ClosedProfessionals)
            {
                query = query.Where(x => x.ContractStatusId == (short)eContractStatus.Closed);
            }
            else if (viewModel.Filter.ProfessionalFilter == eProfessionalFilter.ClosedProfessionals)
            {
                query = query.Where(x => x.ContractStatusId == (short)eContractStatus.Closed);
            }
            // 4- Doctors
            else if (viewModel.Filter.ProfessionalFilter == eProfessionalFilter.Doctors)
            {
                query = query.Where(x => x.ContractStatusId == (short)eContractStatus.Active && x.Field.Equals("MEDICAL", StringComparison.CurrentCultureIgnoreCase));
            }

            // 5- PHYSIOTHERAPISTS 
            else if (viewModel.Filter.ProfessionalFilter == eProfessionalFilter.Physiotherapists)
            {
                query = query.Where(x => x.ContractStatusId == (short)eContractStatus.Active && x.Field.Equals("PHYSIOTHERAPY", StringComparison.CurrentCultureIgnoreCase));
            }

            //6- NURSES (FIELD “M” = NURSING” + CONTRACT STATUS “AW” = ACTIVE)
            else if (viewModel.Filter.ProfessionalFilter == eProfessionalFilter.Nurses)
            {
                query = query.Where(x => x.ContractStatusId == (short)eContractStatus.Active && x.Field.Equals("NURSING", StringComparison.CurrentCultureIgnoreCase));
            }
            //7- SPEECH & LANGUAGE(FIELD “M” = SPEECH & LANGUAGE + CONTRACT STATUS “AW” = ACTIVE)
            else if (viewModel.Filter.ProfessionalFilter == eProfessionalFilter.SpeechAndLanguage)
            {
                query = query.Where(x => x.ContractStatusId == (short)eContractStatus.Active && x.Field.Equals("SPEECH & LANGUAGE", StringComparison.CurrentCultureIgnoreCase));
            }
            //8- PSYCHOLOGISTS(FIELD “M” = PSYCHOLOGY + CONTRACT STATUS “AW” = ACTIVE)
            else if (viewModel.Filter.ProfessionalFilter == eProfessionalFilter.Psychologists)
            {
                query = query.Where(x => x.ContractStatusId == (short)eContractStatus.Active && x.Field.Equals("PSYCHOLOGY", StringComparison.CurrentCultureIgnoreCase));
            }
            //9- PODIATRISTS (FIELD “M” = PODIATRY + CONTRACT STATUS “AW” = ACTIVE)
            else if (viewModel.Filter.ProfessionalFilter == eProfessionalFilter.Podiatrists)
            {
                query = query.Where(x => x.ContractStatusId == (short)eContractStatus.Active && x.Field.Equals("PODIATRY", StringComparison.CurrentCultureIgnoreCase));
            }
            //10- ALTERNATIVE MEDICINE (FIELD “M” = ALTERNATIVE MEDICINE + CONTRACT STATUS “AW” = ACTIVE)
            else if (viewModel.Filter.ProfessionalFilter == eProfessionalFilter.Alternativemedicine)
            {
                query = query.Where(x => x.ContractStatusId == (short)eContractStatus.Active && x.Field.Equals("ALTERNATIVE MEDICINE", StringComparison.CurrentCultureIgnoreCase));
            }

            if (!string.IsNullOrEmpty(viewModel.Filter.Search))
            {
                viewModel.Filter.Search = viewModel.Filter.Search.Trim();
                query = query.Where(x =>
                (
                    (!string.IsNullOrEmpty(x.Name) && x.Name.CLower().Contains(viewModel.Filter.Search.CLower()))
                || (!string.IsNullOrEmpty(x.Email) && x.Email.CLower().Contains(viewModel.Filter.Search.CLower()))
                || (!string.IsNullOrEmpty(x.Field) && x.Field.CLower().Contains(viewModel.Filter.Search.CLower()))
                || (!string.IsNullOrEmpty(x.SubCategory) && x.SubCategory.CLower().Contains(viewModel.Filter.Search.CLower()))
                || (!string.IsNullOrEmpty(x.Services) && x.Services.CLower().Contains(viewModel.Filter.Search.CLower()))
                || (!string.IsNullOrEmpty(x.City) && x.City.CLower().Contains(viewModel.Filter.Search.CLower()))
                || (x.ContractDate.HasValue) && x.ContractDate.Value.ToString("dd/MM/YYYY").Contains(viewModel.Filter.Search.CLower()))
                || (x.ContractEndDate.HasValue) && x.ContractEndDate.Value.ToString("dd/MM/YYYY").Contains(viewModel.Filter.Search.CLower())
                );
            }

            switch (viewModel.SortField.ToLower())
            {
                case "name":
                    if (viewModel.SortOrder.Equals("asc"))
                        query = query.OrderBy(x => x.Name);
                    else
                        query = query.OrderByDescending(x => x.Name);
                    break;

                case "email":
                    if (viewModel.SortOrder.Equals("asc"))
                        query = query.OrderBy(x => x.Email);
                    else
                        query = query.OrderByDescending(x => x.Email);
                    break;

                case "fields":
                    if (viewModel.SortOrder.Equals("asc"))
                        query = query.OrderBy(x => x.Field);
                    else
                        query = query.OrderByDescending(x => x.Field);
                    break;
                case "subcategories":
                    if (viewModel.SortOrder.Equals("asc"))
                        query = query.OrderBy(x => x.SubCategory);
                    else
                        query = query.OrderByDescending(x => x.SubCategory);
                    break;
                case "services":
                    if (viewModel.SortOrder.Equals("asc"))
                        query = query.OrderBy(x => x.Services);
                    else
                        query = query.OrderByDescending(x => x.Services);
                    break;
                case "city":
                    if (viewModel.SortOrder.Equals("asc"))
                        query = query.OrderBy(x => x.City);
                    else
                        query = query.OrderByDescending(x => x.City);
                    break;
                case "contrctdate":
                    if (viewModel.SortOrder.Equals("asc"))
                        query = query.OrderBy(x => x.ContractDate);
                    else
                        query = query.OrderByDescending(x => x.ContractDate);
                    break;
                case "contractenddate":
                    if (viewModel.SortOrder.Equals("asc"))
                        query = query.OrderBy(x => x.ContractEndDate);
                    else
                        query = query.OrderByDescending(x => x.ContractEndDate);
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

       
        private string GetServices(long professionalId, List<Service> services, List<ServiceProfessionals> serviceProfessionals)
        {
            var proServices = (from s in services
                               join
                               sp in serviceProfessionals on s.Id equals sp.ServiceId
                               where sp.ProfessionalId == professionalId
                               select s.Name).ToArray();

            return string.Join("<br/> ", proServices);
        }

        public void GetProfessionalById(long professionalId)
        {
            try
            {
                var professional = _professionalRepository.GetByIdWithIncludes(professionalId).FirstOrDefault();
                var professionalServices = _professionalRepository.GetProfessionalServices(professional.Id);

                var viewModel = _mapper.Map<ProfessionalViewModel>(professional);
                viewModel.AssignedTo = GetAssignedUser(viewModel.AssignedToId);
                viewModel.ProfessionalServices = _mapper.Map<IEnumerable<ServiceProfessionalRelationVeiwModel>>(professionalServices);

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

        public void GetFeesForFilterToConnectWithServiceProfessional(long ptRelationRowId, long proRelationRowId)
        {
            _professionalRepository.GetFeesForFilterToConnectWithServiceProfessional(ptRelationRowId, proRelationRowId);
        }

        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }

    }
}