using System.Linq;
using Medelit.Domain.Interfaces;
using Medelit.Domain.Models;
using Medelit.Infra.Data.Context;
using Medelit.Infra.Data.Repository;
using Microsoft.EntityFrameworkCore;

namespace Equinox.Infra.Data.Repository
{
    public class VatRepository : Repository<Vat>, IVatRepository
    {
        public VatRepository(MedelitContext context)
            : base(context)
        {

        }
    }
}
