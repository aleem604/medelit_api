using System.Linq;
using Medelit.Domain.Interfaces;
using Medelit.Domain.Models;
using Medelit.Infra.Data.Context;
using Medelit.Infra.Data.Repository;
using Microsoft.EntityFrameworkCore;

namespace Equinox.Infra.Data.Repository
{
    public class ApplicationMethodRepository : Repository<ApplicationMethod>, IApplicationMethodRepository
    {
        public ApplicationMethodRepository(MedelitContext context)
            : base(context)
        {

        }

        public IQueryable<ContractStatus> GetContractStatus()
        {
            return Db.ContactStatus;
        }

    }
}
