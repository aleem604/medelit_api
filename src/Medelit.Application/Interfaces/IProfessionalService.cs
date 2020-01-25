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
        dynamic DetachProfessionalConnectedService(IEnumerable<long> servieIds, long proId);
    }
}
