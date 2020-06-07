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
using Microsoft.AspNetCore.Hosting;

namespace Medelit.Application
{
    public class FeeService : BaseService, IFeeService
    {
        private readonly IFeeRepository _feeRepository;
        private readonly IMapper _mapper;
        private readonly IMediatorHandler _bus;

        public FeeService(IMapper mapper,
            ApplicationDbContext context,
            IHttpContextAccessor httpContext,
                            IConfiguration configuration,
                            IMediatorHandler bus,
                            IFeeRepository feeRepository,
                            IHostingEnvironment env
                            ) : base(context, httpContext, configuration, env)
        {
            _mapper = mapper;
            _bus = bus;
            _feeRepository = feeRepository;
        }

        public dynamic GetFees()
        {
            return _feeRepository.GetAll().Select(x => new { x.Id, x.FeeCode }).ToList();
        }

        public void GetFeeTags()
        {
            var result = new List<string>();
  
                var tags = _feeRepository.GetPtFees().Select(s => s.Tags).ToList();
                tags.ForEach(t => {
                    if (!string.IsNullOrEmpty(t))
                    {
                        var tagArr = t.Split(',');
                        result.AddRange(tagArr);
                    }
                });
            
            _bus.RaiseEvent(new DomainNotification(GetType().Name, null, result));
        }

        public void FindFees(SearchViewModel viewModel)
        {
            _feeRepository.FindFees(viewModel);
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