﻿using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Medelit.Common;

namespace Medelit.Application
{
    public interface IFeeService : IDisposable
    {
        void SaveFee(FeeViewModel feeViewModel);
        dynamic GetFees();
        void FindFees(SearchViewModel model);
        void UpdateStatus(IEnumerable<FeeViewModel> fees, eRecordStatus status);
        void DeleteFees(IList<FeeViewModel> feeIds);
        void GetFeeById(long feeId, eFeeType feeType);
        void GetConnectedServices(long feeId, eFeeType feeType);
        void GetConnectedProfessionals(long feeId, eFeeType feeType);
        dynamic GetConnectedProfessionalsCustomers(long feeId);
        void DeleteConnectedProfessionals(IEnumerable<long> prosIds, long feeId, eFeeType feeType);
        dynamic GetServicesToConnectWithFee(long feeId);
        void SaveServicesToConnectWithFee(IEnumerable<long> serviceIds, long feeId);
        void GetProfessionalToConnectWithFee(long feeId, eFeeType feeType);
        void SaveProfessionlToConnectWithFee(IEnumerable<long> serviceIds, long feeId, eFeeType feeType);
        void GetServiceToConnect(long feeId, eFeeType feeType);
        void GetProfessionalToConnect(long serviceId, long feeId, eFeeType feeType);
        void AttachNewServiceProfessionalToFee(long serviceId,long ProfessionalId, long feeId, eFeeType feeType);
    }
}
