using System;
using System.Threading.Tasks;
using Medelit.Common;

namespace Medelit.Application
{
    public interface ILeadService : IDisposable
    {
        dynamic GetLeads();
        dynamic FindLeads(SearchViewModel model);
        LeadViewModel GetLeadById(long leadId);
    }
}
