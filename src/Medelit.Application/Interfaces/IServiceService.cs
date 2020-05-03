using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Medelit.Common;
using Medelit.Common.Models;

namespace Medelit.Application
{
    public interface IServiceService : IDisposable
    {
        dynamic GetServices();
        void FindServices(SearchViewModel model);
        ServiceViewModel GetServiceById(long serviceId);
        void SaveService(ServiceViewModel feeViewModel);
        void DeleteServices(IEnumerable<long> list);
        void UpdateStatus(IEnumerable<ServiceViewModel> services, eRecordStatus status);

        #region service professional connect
        void GetServiceConnectedProfessionals(long serviceId);
        void GetProfessionalsWithFeesToConnectWithService(long serviceId);
        void SaveProfessionalsWithFeesToConnectWithService(IEnumerable<long> professionalids, long serviceId);
        void RemoveProfessionalsFromService(IEnumerable<long> model, long serviceId);
        void GetServiceProfessionalFeeRowDetail(long rowId);
        void GetServiceProfessionalFeesForFilter(long rowId);
        void SaveProfessionalServicesFees(ProfessionalConnectedServicesModel model, long rowId);

        #endregion service professional connect

        #region service connect fees
        void GetServiceConnectedFees(long serviceId, eFeeType feeType);
        void GetServiceConnectedFeesToConnect(long serviceId,eFeeType feeType);
        void SaveFeesForService(IEnumerable<long> model,long serviceId, eFeeType feeType);
        void DetachFeeFromService(IEnumerable<long> model,long serviceId, eFeeType feeType);
        #endregion service connect pt fees


        dynamic GetServiceProfessionals(ServicFilterViewModel viewModel);
        void SaveProfessionalServices(IEnumerable<long> proIds, long proId);
        void DetachProfessional(long serviceId, long proId);
        dynamic GetProfessionalRelations(long proId);
        dynamic GetConnectedCustomersInvoicingEntities(long serviceId);
        dynamic GetConnectedBookings(long serviceId);
        dynamic GetConnectedCustomerInvoices(long serviceId);
        dynamic GetConnectedLeads(long serviceId);
        void AddUpdateFeeForService(AddUpdateFeeToServiceViewModel viewModel);
        
    }
}
