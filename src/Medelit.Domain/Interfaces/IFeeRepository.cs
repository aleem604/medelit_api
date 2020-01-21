using Medelit.Domain.Models;
using System.Collections.Generic;

namespace Medelit.Domain.Interfaces
{
    public interface IFeeRepository : IRepository<Fee>
    {
        dynamic GetConnectedProfessionalsCustomers(long feeId);
        dynamic GetConnectedServices(long feeId);
        dynamic GetServicesToConnectWithFee(long feeId);
        void SaveServicesToConnectWithFee(IEnumerable<long> serviceIds, long feeId);
    }
}