using System.Collections.Generic;
using System.Linq;
using Medelit.Common;
using Medelit.Common.Models;
using Medelit.Domain.Models;

namespace Medelit.Domain.Interfaces
{
    public interface IServiceRepository : IRepository<Service>
    {
        void FindServices(SearchViewModel viewModel);
        void RemoveProfessionals(long serviceId);
        Service GetByIdWithIncludes(long serviceId);

        #region service professiona connect
        dynamic GetServiceProfessionals(long proId, long? fieldId, long? categoryId, string tag);
        void AddProfessionalToService(long serviceId, long professionalId);
        void DetachProfessional(long serviceId, long professionalId);
        void GetServiceConnectedProfessionals(long serviceId);
        void GetProfessionalsWithFeesToConnectWithService(long serviceId);
        void SaveProfessionalsWithFeesToConnectWithService(IEnumerable<long> model, long serviceId);
        void RemoveProfessionalsFromService(IEnumerable<long> model, long serviceId);
        void GetServiceProfessionalFeeRowDetail(long rowId);
        void GetServiceProfessionalFeesForFilter(long rowId);
        void SaveProfessionalServicesFees(ProfessionalConnectedServicesModel model, long rowId);

        #endregion service professional connect

        #region service connect fees
        void GetServiceConnectedFees(long serviceId, eFeeType feeType);
        void GetServiceConnectedFeesToConnect(long serviceId, eFeeType feeType);
        void SaveFeesForService(IEnumerable<long> model, long serviceId, eFeeType feeType);
        void DetachFeeFromService(IEnumerable<long> model, long serviceId, eFeeType feeType);
        #endregion service connect pt fees



        dynamic GetProfessionalServicesWithInclude(long professionalId);
        dynamic GetConnectedCustomersInvoicingEntities(long serviceId);
        dynamic GetConnectedBookings(long serviceId);
        dynamic GetConnectedCustomerInvoices(long serviceId);
        dynamic GetConnectedLeads(long serviceId);
        
    }
}