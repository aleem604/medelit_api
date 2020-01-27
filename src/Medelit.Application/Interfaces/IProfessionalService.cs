using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Medelit.Common;

namespace Medelit.Application
{
    public interface IProfessionalService : IDisposable
    {
        dynamic FindProfessionals(SearchViewModel model);
        dynamic GetProfessionals();
        dynamic GetProfessionalById(long professionalId);
        void SaveProvessional(ProfessionalViewModel model);
        void UpdateStatus(IEnumerable<ProfessionalViewModel> pros, eRecordStatus status);
        void DeleteFees(IEnumerable<long> feeIds);
        dynamic GetConnectedCustomers(long proId);
        dynamic GetConnectedBookings(long proId);
        dynamic GetConnectedInvoices(long proId);
        dynamic GetConnectedLeads(long proId);
        dynamic GetProfessionalConnectedServices(long proId);
        void DetachProfessionalConnectedService(IEnumerable<long> servieIds, long proId);
        dynamic GetProfessionalServiceDetail(long serviceId, long proId);
        void SaveProfessionalServiceDetail(FullFeeViewModel model);
        void GetServicesToAttachWithProfessional(long proId);
        void GetServicesForConnectFilter(long proId);
        void AttachServicesToProfessional(IEnumerable<long> serviceIds, long proId);
        void GetFeesForFilterToConnectWithServiceProfessional(long ptRelationRowId, long proRelationRowId);
    }
}
