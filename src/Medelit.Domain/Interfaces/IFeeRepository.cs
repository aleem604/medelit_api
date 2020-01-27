using Medelit.Common;
using Medelit.Domain.Models;
using System.Collections.Generic;

namespace Medelit.Domain.Interfaces
{
    public interface IFeeRepository : IRepository<VFees>
    {
        PtFee GetPtFee(long feeId);
        ProFee GetProFee(long feeId);
        PtFee AddPtFee(PtFee feeModel);
        ProFee AddProFee(ProFee feeModel);
        PtFee UpdatePtFee(PtFee feeModel);
        ProFee UpdateProFee(ProFee feeModel);
        void RemovePtFee(PtFee feeModel);
        void RemoveProFee(ProFee feeModel);

        void GetFeeByIdAndType(long feeId, eFeeType feeType);
        dynamic GetConnectedProfessionalsCustomers(long feeId);
        void DeleteConnectedProfessionals(IEnumerable<long> prosIds, long feeId, eFeeType feeType);
        void GetConnectedServices(long feeId, eFeeType feeType= eFeeType.PTFee);
        void GetConnectedProfessionals(long feeId, eFeeType feeType);
        dynamic GetServicesToConnectWithFee(long feeId);
        void SaveServicesToConnectWithFee(IEnumerable<long> serviceIds, long feeId);
        void GetProfessionalToConnectWithFee(long feeId, eFeeType feeType);
        
        void SaveProfessionlToConnectWithFee(IEnumerable<long> proIds, long feeId, eFeeType feeType);
        void GetServicesToConnect(long feeId, eFeeType feeType);
        void GetProfessionalToConnect(long serviceId, long feeId, eFeeType feeType);
        void AttachNewServiceProfessionalToFee(long serviceId, long professionalId, long feeId, eFeeType feeType);
    }
}