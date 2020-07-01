using Medelit.Common;
using Medelit.Domain.Models;
using System.Collections.Generic;
using System.Linq;

namespace Medelit.Domain.Interfaces
{
    public interface ILeadRepository : IRepository<Lead>
    {
        void SearchLeads(SearchViewModel viewModel);
        IQueryable<Lead> GetAllWithService();
        Lead GetWithInclude(long leadId);
        void RemoveLeadServices(long leadId);
        Customer GetCustomerId(long? fromCustomerId);
        IQueryable<LeadServices> GetLeadServiceRelations();
        void RemoveAll(List<long> leadIds);
        
    }
}