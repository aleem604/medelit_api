using Medelit.Domain.Models;
using System.Linq;

namespace Medelit.Domain.Interfaces
{
    public interface ILeadRepository : IRepository<Lead>
    {
        IQueryable<Lead> GetAllWithService();
        Lead GetWithInclude(long leadId);
        void RemoveLeadServices(long leadId);
        Customer GetCustomerId(long? fromCustomerId);
    }
}