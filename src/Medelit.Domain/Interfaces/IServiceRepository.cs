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
        IQueryable<ServiceProfessionals> GetServiceProfessionals();
        void RemoveProfessionals(long serviceId);
        Service GetByIdWithIncludes(long serviceId);
        IEnumerable<Service> GetAllWithProfessionals();
        dynamic GetProfessionalServices(long proId, long? fieldId, long? categoryId, string tag);
        void AddProfessionalToService(long serviceId, long professionalId);
        void DetachProfessional(long serviceId, long professionalId);
        void GetServiceConnectedProfessionals(long serviceId);
        void GetProfessionalsWithFeesToConnectWithService(long serviceId);
        void SaveProfessionalsWithFeesToConnectWithService(IEnumerable<EditProfessionalServiceFeesModel> model, long serviceId);
        void RemoveProfessionalsFromService(IEnumerable<EditProfessionalServiceFeesModel> model, long serviceId);

        #region service connect pt fees
        void GetServiceConnectedPtFees(long serviceId);
        void GetServiceConnectedPtFeesToConnect(long serviceId);
        void SavePtFeesForService(IEnumerable<ServiceConnectedPtFeesModel> model, long serviceId);
        void DetachPtFeeFromService(IEnumerable<ServiceConnectedPtFeesModel> model, long serviceId);
        #endregion service connect pt fees

        #region service connect pro fees
        void GetServiceConnectedProFees(long serviceId);
        void GetServiceConnectedProFeesToConnect(long serviceId);
        void SaveProFeesForService(IEnumerable<ServiceConnectedProFeesModel> model, long serviceId);
        void DetachProFeeFromService(IEnumerable<ServiceConnectedProFeesModel> model, long serviceId);
        #endregion service connect pt fees



        dynamic GetProfessionalServicesWithInclude(long professionalId);
        dynamic GetConnectedCustomersInvoicingEntities(long serviceId);
        dynamic GetConnectedBookings(long serviceId);
        dynamic GetConnectedCustomerInvoices(long serviceId);
        dynamic GetConnectedLeads(long serviceId);
        void AddUpdateFeeForService(AddUpdateFeeToService viewModel);
        
    }
}