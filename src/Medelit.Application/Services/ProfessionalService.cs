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
                Field = s.Field.Field,
                SubCategory = s.SubCategory.SubCategory,
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

            switch (viewModel.SortField)
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

                case "field":
                    if (viewModel.SortOrder.Equals("asc"))
                        query = query.OrderBy(x => x.Field);
                    else
                        query = query.OrderByDescending(x => x.Field);
                    break;
                case "subcategory":
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

        private string GetProfessionalFields(long professionalId, List<FieldSubCategory> fields, List<Service> services, List<ServiceProfessionalRelation> serviceProfessionalRelations)
        {

            var proFields = (from f in fields
                             join
                             s in services on f.Id equals s.FieldId
                             join
                             sp in serviceProfessionalRelations on s.Id equals sp.ServiceId
                             where sp.ProfessionalId == professionalId
                             select f.Field).ToArray();

            return string.Join(", ", proFields);
        }

        private string GetProfessionalCats(long professionalId, List<FieldSubCategory> fields, List<Service> services, List<ServiceProfessionalRelation> serviceProfessionalRelations)
        {
            var proCats = (from f in fields
                           join
                           s in services on f.Id equals s.FieldId
                           join
                           sp in serviceProfessionalRelations on s.Id equals sp.ServiceId
                           where sp.ProfessionalId == professionalId
                           select f.SubCategory).ToArray();

            return string.Join(", ", proCats);
        }

        private string GetServices(long professionalId, List<Service> services, List<ServiceProfessionalRelation> serviceProfessionals)
        {
            var proServices = (from s in services
                               join
                               sp in serviceProfessionals on s.Id equals sp.ServiceId
                               where sp.ProfessionalId == professionalId
                               select s.Name).ToArray();

            return string.Join(", ", proServices);
        }

        public dynamic GetProfessionalById(long professionalId)
        {
            var professional = _professionalRepository.GetByIdWithIncludes(professionalId).FirstOrDefault();
            var professionalServices = _professionalRepository.GetProfessionalServices(professional.Id);

            var viewModel = _mapper.Map<ProfessionalViewModel>(professional);
            viewModel.Languages = professional.ProfessionalLangs.Select((s) => new FilterModel { Id = s.LanguageId }).ToList();
            viewModel.AssignedTo = GetAssignedUser(viewModel.AssignedToId);
            viewModel.ProfessionalServices = _mapper.Map<IEnumerable<ServiceProfessionalRelationVeiwModel>>(professionalServices);

            return viewModel;

        }

        public void SaveProvessional(ProfessionalViewModel model)
        {
            var professionalModel = _mapper.Map<Professional>(model);
            var proLangModel = new List<ProfessionalLanguages>();
            foreach (var lang in model.Languages)
            {
                proLangModel.Add(new ProfessionalLanguages
                {
                    LanguageId = (int)lang.Id
                });
            }

            professionalModel.ProfessionalLangs = proLangModel;
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

        public dynamic GetProfessionalConnectedServices(long proId)
        {
            return _professionalRepository.GetProfessionalConnectedServices(proId);
        }

        public dynamic DetachProfessionalConnectedService(IEnumerable<long> serviceIds, long proId)
        {
            return _professionalRepository.DetachProfessionalConnectedService(serviceIds, proId);
        }

        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }

    }
}