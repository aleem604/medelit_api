using Medelit.Common.Models;
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
        IEnumerable<ServiceProfessionalPtFees> GetServiceProfessionalPtFees(long id);
        IEnumerable<ServiceProfessionalProFees> GetServiceProfessionalProFees(long id);
        dynamic GetConnectedCustomers(long proId);
        dynamic GetConnectedBookings(long proId);
        dynamic GetConnectedInvoices(long proId);
        dynamic GetConnectedLeads(long proId);
        dynamic GetProfessionalConnectedServices(long proId);
        void DetachProfessionalConnectedService(IEnumerable<long> serviceIds, long proId);
        dynamic GetProfessionalServiceDetail(long serviceId, long proId);
        void SaveProfessionalServiceDetail(EditProfessionalServiceFeesModel model);
        void GetServicesToAttachWithProfessional(long proId);
        void GetServicesForConnectFilter(long proId);
        void AttachServicesToProfessional(IEnumerable<long> serviceIds, long proId);
        void GetFeesForFilterToConnectWithServiceProfessional(long ptRelationRowId, long proRelationRowId);
    }
}