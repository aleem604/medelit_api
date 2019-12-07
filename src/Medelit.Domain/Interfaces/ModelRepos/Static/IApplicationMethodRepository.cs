using Medelit.Domain.Models;
using System.Linq;

namespace Medelit.Domain.Interfaces
{
    public interface IApplicationMethodRepository : IRepository<ApplicationMethod>
    {
        IQueryable<ContractStatus> GetContractStatus();
    }
}