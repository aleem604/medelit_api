using System.Linq;
using Medelit.Domain.Interfaces;
using Medelit.Domain.Models;
using Medelit.Infra.Data.Context;
using Medelit.Infra.Data.Repository;
using Microsoft.EntityFrameworkCore;

namespace Medelit.Infra.Data.Repository
{
    public class InvoiceEntityRepository : Repository<InvoiceEntity>, IInvoiceEntityRepository
    {
        public InvoiceEntityRepository(MedelitContext context)
            : base(context)
        {

        }
    }
}
