using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Medelit.Common;

namespace Medelit.Application
{
    public interface IServiceService : IDisposable
    {
        dynamic GetServices();
        dynamic FindServices(SearchViewModel model);
        ServiceViewModel GetServiceById(long serviceId);
        void SaveService(ServiceViewModel feeViewModel);
        void DeleteServices(IEnumerable<long> list);
        void UpdateStatus(IEnumerable<ServiceViewModel> services, eRecordStatus status);
    }
}
