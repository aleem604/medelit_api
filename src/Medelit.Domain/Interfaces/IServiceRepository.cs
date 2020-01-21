using System.Collections.Generic;
using System.Linq;
using Medelit.Domain.Models;

namespace Medelit.Domain.Interfaces
{
    public interface IServiceRepository : IRepository<Service>
    {
        IQueryable<ServiceProfessionalRelation> GetServiceProfessionals();
        void RemoveProfessionals(long serviceId);
        Service GetByIdWithIncludes(long serviceId);
        IEnumerable<Service> GetAllWithProfessionals();
        dynamic GetProfessionalServices(long proId, long? fieldId, long? categoryId, string tag);
        void AddProfessionalToService(long serviceId, long professionalId);
        void DetachProfessional(long serviceId, long professionalId);
        dynamic GetProfessionalServicesWithInclude(long professionalId);
        dynamic GetProfessionalFeesDetail(long serviceId);
        dynamic GetServiceConnectedProfessionals(long serviceId);
        dynamic GetConnectedCustomersInvoicingEntities(long serviceId);
        dynamic GetConnectedBookings(long serviceId);
        dynamic GetConnectedCustomerInvoices(long serviceId);
        dynamic GetConnectedLeads(long serviceId);
        void AddUpdateFeeForService(AddUpdateFeeToService viewModel);
    }
}