using System.Linq;
using Medelit.Domain.Interfaces;
using Medelit.Domain.Models;
using Medelit.Infra.Data.Context;
using Medelit.Infra.Data.Repository;
using Microsoft.EntityFrameworkCore;

namespace Equinox.Infra.Data.Repository
{
    public class LeadStatusRepository : Repository<LeadStatus>, ILeadStatusRepository
    {
        public LeadStatusRepository(MedelitContext context)
            : base(context)
        {

        }
    }
}
