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
using Medelit.Domain.Core.Notifications;

namespace Medelit.Application
{
    public class FeeService : BaseService, IFeeService
    {
        private readonly IFeeRepository _feeRepository;
        private readonly IMapper _mapper;
        private readonly IMediatorHandler _bus;

        public FeeService(IMapper mapper,
            ApplicationDbContext context,
                            IMediatorHandler bus,
                            IFeeRepository feeRepository) : base(context)
        {
            _mapper = mapper;
            _bus = bus;
            _feeRepository = feeRepository;
        }

        public dynamic GetFees()
        {
            return _feeRepository.GetAll().Select(x => new { x.Id, x.FeeCode }).ToList();
        }

        public void FindFees(SearchViewModel viewModel)
        {
            try
            {
                viewModel.Filter = viewModel.Filter ?? new SearchFilterViewModel();

                var query = _feeRepository.GetAll().Where(x => x.Status != eRecordStatus.Deleted).Select((x) => new
                {
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

                var result = new
                {
                    items = query.Skip(viewModel.PageNumber * viewModel.PageSize).Take(viewModel.PageSize).ToList(),
                    totalCount
                };
                _bus.RaiseEvent(new DomainNotification(GetType().Name, null, result));

            }
            catch (Exception ex)
            {
                _bus.RaiseEvent(new DomainNotification(GetType().Name, ex.Message));
            }
        }

        public void GetFeeById(long feeId, eFeeType feeType)
        {
            _feeRepository.GetFeeByIdAndType(feeId, feeType);
        }

        public void SaveFee(FeeViewModel viewModel)
        {
            var feeModel = _mapper.Map<PtFee>(viewModel);
            _bus.SendCommand(new SaveFeeCommand { Fee = feeModel });
        }

        public void UpdateStatus(IEnumerable<FeeViewModel> fees, eRecordStatus status)
        {
            //_bus.SendCommand(new UpdateFeesStatusCommand { Fees = _mapper.Map<IEnumerable<FeeViewModel>, IEnumerable<VFees>>(fees), Status = status });
        }

        public void DeleteFees(IList<FeeViewModel> fees)
        {
            _bus.SendCommand(new DeleteFeesCommand { VFees = _mapper.Map<IEnumerable<FeeViewModel>, IEnumerable<VFees>>(fees) });
        }

        public void ConnectFeesToServiceProfessional(IEnumerable<FeeViewModel> fees, long serviceId, long professionalId)
        {
            var ptFees = _mapper.Map<IEnumerable<PtFee>>(fees.Where(x=>x.FeeTypeId == eFeeType.PTFee));
            var proFees = _mapper.Map<IEnumerable<ProFee>>(fees.Where(x=>x.FeeTypeId == eFeeType.PROFee));

            _feeRepository.ConnectFeesToServiceProfessional(ptFees, proFees, serviceId, professionalId);
        }


        public void GetFeeConnectedServices(long feeId, eFeeType feeType)
        {
            _feeRepository.GetFeeConnectedServices(feeId, feeType);
        }

        public void GetConnectedProfessionals(long feeId, eFeeType feeType)
        {
            _feeRepository.GetConnectedProfessionals(feeId, feeType);
        }


        public void DeleteConnectedProfessionals(IEnumerable<FeeConnectedProfessionalsViewModel> prosIds, long feeId, eFeeType feeType)
        {
            _feeRepository.DeleteConnectedProfessionals(prosIds, feeId, feeType);
        }

        public dynamic GetServicesToConnectWithFee(long feeId, eFeeType feeType)
        {
            return _feeRepository.GetServicesToConnectWithFee(feeId, feeType);
        }

        public void SaveServicesToConnectWithFee(IEnumerable<long> serviceIds, long feeId, eFeeType feeType)
        {
            _feeRepository.SaveServicesToConnectWithFee(serviceIds, feeId, feeType);
        }

        public void GetProfessionalToConnectWithFee(long feeId, eFeeType feeType)
        {
            _feeRepository.GetProfessionalToConnectWithFee(feeId, feeType);
        }
        public void SaveProfessionlToConnectWithFee(IEnumerable<long> serviceProFeeIds, long feeId, eFeeType feeType)
        {
            _feeRepository.SaveProfessionlToConnectWithFee(serviceProFeeIds, feeId, feeType);
        }

        public void GetServiceToConnect(long feeId, eFeeType feeType)
        {
            _feeRepository.GetServicesToConnect(feeId, feeType);
        }

        public void GetServiceProfessionalsForFilter(long serviceId, long feeId, eFeeType feeType)
        {
            _feeRepository.GetServiceProfessionalsForFilter(serviceId, feeId, feeType);
        }

        public void AttachNewServiceProfessionalToFee(long serviceId, long professionalId, long feeId, eFeeType feeType)
        {
            _feeRepository.AttachNewServiceProfessionalToFee(serviceId, professionalId, feeId, feeType);
        }

        public void DeleteConnectedServices(IEnumerable<long> serviceProFeeIds, long feeId, eFeeType feedType)
        {
            _feeRepository.DeleteConnectedServices(serviceProFeeIds, feeId, feedType);
        }

        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }


    }
}