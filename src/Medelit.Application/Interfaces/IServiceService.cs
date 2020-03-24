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
        void GetServiceConnectedProfessionals(long serviceId);
        void GetProfessionalsWithFeesToConnectWithService(long serviceId);
        void SaveProfessionalsWithFeesToConnectWithService(IEnumerable<EditProfessionalServiceFeesModel> model, long serviceId);
        void RemoveProfessionalsFromService(IEnumerable<EditProfessionalServiceFeesModel> model, long serviceId);

        #region service connect pt fees
        void GetServiceConnectedPtFees(long serviceId);
        void GetServiceConnectedPtFeesToConnect(long serviceId);
        void SavePtFeesForService(IEnumerable<ServiceConnectedPtFeesModel> model,long serviceId);
        void DetachPtFeeFromService(IEnumerable<ServiceConnectedPtFeesModel> model,long serviceId);
        #endregion service connect pt fees

        #region service connect pro fees
        void GetServiceConnectedProFees(long serviceId);
        void GetServiceConnectedProFeesToConnect(long serviceId);
        void SaveProFeesForService(IEnumerable<ServiceConnectedProFeesModel> model, long serviceId);
        void DetachProFeeFromService(IEnumerable<ServiceConnectedProFeesModel> model, long serviceId);
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
