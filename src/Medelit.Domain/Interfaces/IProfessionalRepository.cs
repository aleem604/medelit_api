using Medelit.Domain.Models;
using System.Collections.Generic;
using System.Linq;

namespace Medelit.Domain.Interfaces
{
    public interface IProfessionalRepository : IRepository<Professional>
    {
        IQueryable<ProfessionalLanguages> GetAllLangs();
        IQueryable<Professional> GetByIdWithIncludes(long professionalId);
        void DeleteLangs(long id);
        IEnumerable<ServiceProfessionalRelation> GetProfessionalServices(long id);
        dynamic GetConnectedCustomers(long proId);
        dynamic GetConnectedBookings(long proId);
        dynamic GetConnectedInvoices(long proId);
        dynamic GetConnectedLeads(long proId);
        dynamic GetProfessionalConnectedServices(long proId);
        dynamic DetachProfessionalConnectedService(IEnumerable<long> serviceIds, long proId);
    }
}