using System;
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
using Microsoft.AspNet.Identity;
using Medelit.Infra.CrossCutting.Identity.Models;
using Medelit.Infra.CrossCutting.Identity.Data;

namespace Medelit.Application
{
    public class LeadService : BaseService, ILeadService
    {
        private readonly ILeadRepository _leadRepository;
        private readonly IStaticDataRepository _staticRepository;
        private readonly ILanguageRepository _langRepository;
        private readonly IServiceRepository _serviceRepository;
        private readonly IProfessionalRepository _professoinalRepository;
        private readonly IMapper _mapper;
        private readonly IConfiguration _configuration;
        private readonly IHttpContextAccessor _httpContext;
        private readonly IMediatorHandler _bus;

        public LeadService(IMapper mapper,
            ApplicationDbContext context,
                            IHttpContextAccessor httpContext,
                            IConfiguration configuration,
                            IMediatorHandler bus,
                            ILeadRepository leadRepository,
                            ILanguageRepository langRepository,
                            IStaticDataRepository staticRepository,
                            IServiceRepository serviceRepository,
                            IProfessionalRepository professionalRepository

            ):base(context)
        {
            _mapper = mapper;
            _httpContext = httpContext;
            _configuration = configuration;
            _bus = bus;
            _leadRepository = leadRepository;
            _staticRepository = staticRepository;
            _langRepository = langRepository;
            _serviceRepository = serviceRepository;
            _professoinalRepository = professionalRepository;
        }

        public dynamic GetLeads()
        {
            return _leadRepository.GetAll().Select(x => new { x.Id, x.TitleId, x.SurName }).ToList();
        }

        public dynamic FindLeads(SearchViewModel viewModel)
        {
            viewModel.Filter = viewModel.Filter ?? new SearchFilterViewModel();
            var langs = _langRepository.GetAll().ToList();
            var statics = _staticRepository.GetAll().ToList();
            //var services = _serviceRepository.GetAll().Select(x => new FilterModel { Id = x.Id, Value = x.Name }).ToList();
            //var professionals = _professoinalRepository.GetAll().Select(x => new FilterModel { Id = x.Id, Value = x.Name }).ToList();


            var query = (from lead in _leadRepository.GetAllWithService()
                         where lead.ConvertDate == null && lead.Status != eRecordStatus.Deleted
                         select lead)
                        .Select((x) => new
                        {
                            x.Id,
                            Language = langs.FirstOrDefault(s => s.Id == x.LanguageId).Name,
                            //Services = PopulateServices(x.Services, services),
                            //Professionals = PopulateProfessionals(x.Services, professionals),
                            //PtFees = PopulatePtFees(x.Services, services),
                            //ProFees = PopulateProFees(x.Services, services),
                            x.LeadStatusId,
                            x.SurName,
                            x.Name,
                            x.Email,
                            x.MainPhone,
                            x.CreateDate,
                            AssignedTo = GetAssignedUser(x.AssignedToId)
                        });

            if (!string.IsNullOrEmpty(viewModel.Filter.Search))
            {
                viewModel.Filter.Search = viewModel.Filter.Search.Trim();
                query = query.Where(x =>
                (
                    (!string.IsNullOrEmpty(x.Name) && x.Name.CLower().Contains(viewModel.Filter.Search.CLower()))
                || (!string.IsNullOrEmpty(x.SurName) && x.SurName.CLower().Contains(viewModel.Filter.Search.CLower()))
                || (!string.IsNullOrEmpty(x.MainPhone) && x.MainPhone.CLower().Contains(viewModel.Filter.Search.CLower()))
                //|| (!string.IsNullOrEmpty(x.Services) && x.Services.CLower().Contains(viewModel.Filter.Search.CLower()))
                //|| (!string.IsNullOrEmpty(x.Professionals) && x.Professionals.CLower().Contains(viewModel.Filter.Search.CLower()))
                || (x.CreateDate.ToString("dd/MM/yyyy").Contains(viewModel.Filter.Search.CLower()))
                || (x.Id.ToString().Contains(viewModel.Filter.Search))

                ));
            }

            if (viewModel.Filter.Filter == eLeadsFilter.HotLeads)
            {
                query = query.Where(x => x.LeadStatusId == (int)eLeadsStatus.Hot);
            }

            switch (viewModel.SortField.ToLower())
            {
                case "name":
                    if (viewModel.SortOrder.Equals("asc"))
                        query = query.OrderBy(x => x.Name);
                    else
                        query = query.OrderByDescending(x => x.Name);
                    break;

                case "surname":
                    if (viewModel.SortOrder.Equals("asc"))
                        query = query.OrderBy(x => x.SurName);
                    else
                        query = query.OrderByDescending(x => x.SurName);
                    break;

                case "email":
                    if (viewModel.SortOrder.Equals("asc"))
                        query = query.OrderBy(x => x.Email);
                    else
                        query = query.OrderByDescending(x => x.Email);
                    break;
                case "language":
                    if (viewModel.SortOrder.Equals("asc"))
                        query = query.OrderBy(x => x.Language);
                    else
                        query = query.OrderByDescending(x => x.Language);
                    break;

                case "createDate":
                    if (viewModel.SortOrder.Equals("asc"))
                        query = query.OrderBy(x => x.CreateDate);
                    else
                        query = query.OrderByDescending(x => x.CreateDate);
                    break;
                //case "createdBy":
                //    if (viewModel.SortOrder.Equals("asc"))
                //        query = query.OrderBy(x => x.CreatedBy);
                //    else
                //        query = query.OrderByDescending(x => x.CreatedBy);
                //    break;

                default:
                    if (viewModel.SortOrder.Equals("asc"))
                        query = query.OrderBy(x => x.AssignedTo);
                    else
                        query = query.OrderByDescending(x => x.AssignedTo);

                    break;
            }

            var totalCount = query.LongCount();

            return new
            {
                items = query.Skip(viewModel.PageNumber * viewModel.PageSize).Take(viewModel.PageSize).ToList(),
                totalCount
            };
        }

        private string PopulateServices(ICollection<LeadServiceRelation> services, List<FilterModel> oservices)
        {
            var query = from s in services
                        join
                        os in oservices on s.ServiceId equals os.Id
                        select os.Value;

            return string.Join(", ", query.ToArray());
        }

        private string PopulatePtFees(ICollection<LeadServiceRelation> services, List<FilterModel> oservices)
        {
            var query = (from s in services
                         join
                        os in oservices on s.ServiceId equals os.Id
                         select new
                         {
                             PtFees = s.IsPtFeeA1 == 1 ? s.PTFeeA1 : s.PTFeeA2
                         }).ToList();

            return string.Join(", ", query.Select(x => x.PtFees.HasValue ? x.PtFees.Value.ToString("G29") : "").ToArray());
        }

        private string PopulateProFees(ICollection<LeadServiceRelation> services, List<FilterModel> oservices)
        {
            var query = (from s in services
                         join
                        os in oservices on s.ServiceId equals os.Id
                         select new
                         {
                             PtFees = s.IsProFeeA1 == 1 ? s.PROFeeA1 : s.PROFeeA2
                         }).ToList();

            return string.Join(", ", query.Select(x => x.PtFees.HasValue ? x.PtFees.Value.ToString("G29") : "").ToArray());
        }

        private string PopulateProfessionals(ICollection<LeadServiceRelation> services, List<FilterModel> professionals)
        {
            var query = from s in services
                        join
                        os in professionals on s.ProfessionalId equals os.Id
                        select os.Value;

            return string.Join(", ", query.ToArray());
        }

        public LeadViewModel GetLeadById(long leadId, long? fromCustomerId)
        {
            if (fromCustomerId.HasValue)
            {
                var customerObj = _leadRepository.GetCustomerId(fromCustomerId);
                var model = _mapper.Map<LeadViewModel>(customerObj);
                model.CustomerId = customerObj.Id;
                model.Customer = customerObj.Name;
                model.Id = leadId;
                model.AssignedTo = GetAssignedUser(model.AssignedToId);
                return model;
            }
            else
            {
                var model = _leadRepository.GetWithInclude(leadId);
                var vm = _mapper.Map<LeadViewModel>(model);
                vm.AssignedTo = GetAssignedUser(vm.AssignedToId);

                return vm;
            }
        }

        public void SaveLead(LeadViewModel viewModel)
        {
            var leadModel = _mapper.Map<Lead>(viewModel);

            leadModel.Services = _mapper.Map<ICollection<LeadServiceRelation>>(viewModel.Services);
            _bus.SendCommand(new SaveLeadCommand { Lead = leadModel, FromCustomerId = viewModel.CustomerId });
        }

        public void UpdateStatus(IEnumerable<LeadViewModel> leads, eRecordStatus status)
        {
            _bus.SendCommand(new UpdateLeadsStatusCommand { Leads = _mapper.Map<IEnumerable<Lead>>(leads), Status = status });
        }

        public void DeleteLeads(IEnumerable<long> leadIds)
        {
            _bus.SendCommand(new DeleteLeadsCommand { LeadIds = leadIds });
        }

        public void ConvertToBooking(long leadId)
        {
            _bus.SendCommand(new ConvertLeadToBookingCommand { LeadId = leadId });
        }

        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }

    }
}