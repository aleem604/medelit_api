using Medelit.Common;
using Medelit.Domain.Models;
using System.Collections.Generic;
using System.Linq;

namespace Medelit.Domain.Interfaces
{
    public interface IFeeRepository : IRepository<VFees>
    {
        IQueryable<PtFee> GetPtFees();
        IQueryable<ProFee> GetProFees();
        PtFee GetPtFee(long feeId);
        ProFee GetProFee(long feeId);
        PtFee AddPtFee(PtFee feeModel);
        ProFee AddProFee(ProFee feeModel);
        PtFee UpdatePtFee(PtFee feeModel);
        ProFee UpdateProFee(ProFee feeModel);
        void RemovePtFee(PtFee feeModel);
        void RemoveProFee(ProFee feeModel);

        void GetFeeByIdAndType(long feeId, eFeeType feeType);
        void ConnectFeesToServiceProfessional(IEnumerable<PtFee> ptFees, IEnumerable<ProFee> proFees, long serviceId, long professionalId);
        void DeleteConnectedProfessionals(IEnumerable<FeeConnectedProfessionalsViewModel> prosIds, long feeId, eFeeType feeType);
        void GetFeeConnectedServices(long feeId, eFeeType feeType= eFeeType.PTFee);
        void GetConnectedProfessionals(long feeId, eFeeType feeType);
        dynamic GetServicesToConnectWithFee(long feeId, eFeeType feeType);
        void SaveServicesToConnectWithFee(IEnumerable<long> serviceIds, long feeId, eFeeType feeType);
        void GetProfessionalToConnectWithFee(long feeId, eFeeType feeType);
        
        void SaveProfessionlToConnectWithFee(IEnumerable<long> serviceProFeeIds, long feeId, eFeeType feeType);
        void GetServicesToConnect(long feeId, eFeeType feeType);
        void GetServiceProfessionalsForFilter(long serviceId, long feeId, eFeeType feeType);
        void AttachNewServiceProfessionalToFee(long serviceId, long professionalId, long feeId, eFeeType feeType);
        void DeleteConnectedServices(IEnumerable<long> serviceProFeeIds, long feeId, eFeeType feedType);
       
    }
}