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
    public class FeeService : BaseService, IFeeService
    {
        private readonly IFeeRepository _feeRepository;
        private readonly ILanguageRepository _langRepository;
        private readonly IStaticDataRepository _staticRepository;
        private readonly IFieldSubcategoryRepository _fieldSubcategoryRepository;
        private readonly IServiceRepository _serviceRepository;
        private readonly IProfessionalRepository _professionalRepository;
        private readonly IMapper _mapper;
        private readonly IConfiguration _configuration;
        private readonly IHttpContextAccessor _httpContext;
        private readonly IMediatorHandler _bus;

        public FeeService(IMapper mapper,
            ApplicationDbContext context,
                            IHttpContextAccessor httpContext,
                            IConfiguration configuration,
                            IMediatorHandler bus,
                            IFeeRepository feeRepository,
                            ILanguageRepository langRepository,
                            IStaticDataRepository staticRepository,
                            IFieldSubcategoryRepository fieldSubcategoryRepository,
                            IServiceRepository serviceRepository,
                            IProfessionalRepository professionalRepository

            ):base(context)
        {
            _mapper = mapper;
            _httpContext = httpContext;
            _configuration = configuration;
            _bus = bus;
            _feeRepository = feeRepository;
            _langRepository = langRepository;
            _staticRepository = staticRepository;
            _fieldSubcategoryRepository = fieldSubcategoryRepository;
            _serviceRepository = serviceRepository;
            _professionalRepository = professionalRepository;
        }
       
       

        public dynamic GetFees()
        {
            return _feeRepository.GetAll().Select(x=> new {x.Id, x.FeeCode }).ToList();
        }

        public dynamic FindFees(SearchViewModel viewModel)
        {
            viewModel.Filter = viewModel.Filter ?? new SearchFilterViewModel();
         
            var query = _feeRepository.GetAll().Where(x => x.Status != eRecordStatus.Deleted).Select((x) => new {
                x.Id,
                x.FeeName,
                x.FeeTypeId,
                FeeType = x.FeeTypeId.GetDescription(),
                x.FeeCode,
                x.Tags,
                x.A1,
                x.A2,
                x.CreateDate,
                x.UpdateDate,
                AssignedTo = GetAssignedUser(x.AssignedToId)
            });
           
            if (!string.IsNullOrEmpty(viewModel.Filter.Search))
            {
                viewModel.Filter.Search = viewModel.Filter.Search.Trim();
                query = query.Where(x =>
                (
                    (!string.IsNullOrEmpty(x.FeeName) && x.FeeName.CLower().Contains(viewModel.Filter.Search.CLower()))
                || (!string.IsNullOrEmpty(x.FeeCode) && x.FeeCode.CLower().Contains(viewModel.Filter.Search.CLower()))
                || (!string.IsNullOrEmpty(x.FeeType) && x.FeeType.CLower().Contains(viewModel.Filter.Search.CLower()))
                || (!string.IsNullOrEmpty(x.A1.ToString()) && x.A1.ToString().CLower().Contains(viewModel.Filter.Search.CLower()))
                || (x.CreateDate.ToString("dd/MM/yyyy").CLower().Contains(viewModel.Filter.Search.CLower()))
                || (x.UpdateDate.HasValue && x.UpdateDate.Value.ToString("dd/MM/yyyy").CLower().Contains(viewModel.Filter.Search.CLower()))
                || (!string.IsNullOrEmpty(x.AssignedTo) && x.AssignedTo.ToString().CLower().Contains(viewModel.Filter.Search.CLower()))
                || (!string.IsNullOrEmpty(x.Tags) && x.Tags.ToString().CLower().Contains(viewModel.Filter.Search.CLower()))
                || (x.Id.ToString().Contains(viewModel.Filter.Search))

                ));
            }

            if (viewModel.Filter.FeeType != eFeeType.All)
            {
                query = query.Where(x => x.FeeTypeId == viewModel.Filter.FeeType);
            }

            switch (viewModel.SortField)
            {
                case "feename":
                    if (viewModel.SortOrder.Equals("asc"))
                        query = query.OrderBy(x => x.FeeName);
                    else
                        query = query.OrderByDescending(x => x.FeeName);
                    break;

                case "feetype":
                    if (viewModel.SortOrder.Equals("asc"))
                        query = query.OrderBy(x => x.FeeType);
                    else
                        query = query.OrderByDescending(x => x.FeeType);
                    break;

                case "feecode":
                    if (viewModel.SortOrder.Equals("asc"))
                        query = query.OrderBy(x => x.FeeCode);
                    else
                        query = query.OrderByDescending(x => x.FeeCode);
                    break;
                case "a1":
                    if (viewModel.SortOrder.Equals("asc"))
                        query = query.OrderBy(x => x.A1);
                    else
                        query = query.OrderByDescending(x => x.A1);
                    break;
                case "a2":
                    if (viewModel.SortOrder.Equals("asc"))
                        query = query.OrderBy(x => x.A2);
                    else
                        query = query.OrderByDescending(x => x.A2);
                    break;

                default:
                    if (viewModel.SortOrder.Equals("asc"))
                        query = query.OrderBy(x => x.FeeCode);
                    else
                        query = query.OrderByDescending(x => x.FeeCode);

                    break;
            }

            var totalCount = query.LongCount();

            return new
            {
                items = query.Skip(viewModel.PageNumber * viewModel.PageSize).Take(viewModel.PageSize).ToList(),
                totalCount
            };
        }

        public FeeViewModel GetFeeById(long feeId)
        {
            var feeModel = _feeRepository.GetById(feeId);
            var vm = _mapper.Map<FeeViewModel>(feeModel);
            vm.FeeType = vm.FeeTypeId.GetDescription();
            return vm;
        }

        public void SaveFee(FeeViewModel viewModel)
        {
            var feeModel = _mapper.Map<Fee>(viewModel);
            _bus.SendCommand(new SaveFeeCommand { Fee = feeModel });
        }

        public void UpdateStatus(IList<FeeViewModel> fees, eRecordStatus status)
        {
            _bus.SendCommand(new UpdateFeesStatusCommand { Fees = _mapper.Map<IEnumerable<Fee>>(fees), Status = status });
        }

        public void DeleteFees(IList<long> feeIds)
        {
            _bus.SendCommand(new DeleteFeesCommand { FeeIds = feeIds });
        }

        public dynamic GetConnectedServices(long feeId)
        {
            return _feeRepository.GetConnectedServices(feeId);
        }

        public dynamic GetConnectedProfessionals(long feeId)
        {
            return _feeRepository.GetConnectedProfessionals(feeId);
        }


        public dynamic GetConnectedProfessionalsCustomers(long feeId)
        {
            return _feeRepository.GetConnectedProfessionalsCustomers(feeId);
        }

        public void DeleteConnectedProfessionals(IEnumerable<long> prosIds, long feeId)
        {
            _feeRepository.DeleteConnectedProfessionals(prosIds, feeId);
        }

        public dynamic GetServicesToConnectWithFee(long feeId)
        {
            return _feeRepository.GetServicesToConnectWithFee(feeId);
        }

        public void SaveServicesToConnectWithFee(IEnumerable<long> serviceIds, long feeId)
        {
            _feeRepository.SaveServicesToConnectWithFee(serviceIds, feeId);
        }

        public void GetProfessionalToConnectWithFee(long feeId)
        {
            _feeRepository.GetProfessionalToConnectWithFee(feeId);
        }
        public void SaveProfessionlToConnectWithFee(IEnumerable<long> proIds, long feeId)
        {
            _feeRepository.SaveProfessionlToConnectWithFee(proIds, feeId);
        }

        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }

        
    }
}