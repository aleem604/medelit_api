using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Medelit.Common;

namespace Medelit.Application
{
    public interface ILeadService : IDisposable
    {
        dynamic GetLeads();
        dynamic FindLeads(SearchViewModel model);
        void SearchLeads(SearchViewModel model);
        LeadViewModel GetLeadById(long leadId, long? fromCustomerId);
        void SaveLead(LeadViewModel model);
        void UpdateStatus(IEnumerable<LeadViewModel> leads, eRecordStatus status);
        void DeleteLeads(IEnumerable<long> leadIds);
        void ConvertToBooking(long leadId);
        void LeadsBulkUpload(IEnumerable<LeadCSVViewModel> leads);
    }
}
