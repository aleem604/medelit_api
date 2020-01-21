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
        dynamic GetProfessionalServices(ServicFilterViewModel viewModel);
        void SaveProfessionalServices(IEnumerable<long> proIds, long proId);
        void DetachProfessional(long serviceId, long proId);
        dynamic GetProfessionalRelations(long proId);
        dynamic GetProfessionalFeesDetail(long serviceId);
        dynamic GetServiceConnectedProfessionals(long serviceId);
        dynamic GetConnectedCustomersInvoicingEntities(long serviceId);
        dynamic GetConnectedBookings(long serviceId);
        dynamic GetConnectedCustomerInvoices(long serviceId);
        dynamic GetConnectedLeads(long serviceId);
        void AddUpdateFeeForService(AddUpdateFeeToServiceViewModel viewModel);
    }
}
