using System.Linq;
using Medelit.Domain.Interfaces;
using Medelit.Domain.Models;
using Medelit.Infra.Data.Context;
using Medelit.Infra.Data.Repository;
using Microsoft.EntityFrameworkCore;

namespace Equinox.Infra.Data.Repository
{
    public class InvoiceRepository : Repository<Invoice>, IInvoiceRepository
    {
        public InvoiceRepository(MedelitContext context)
            : base(context)
        {
        }
        public DbSet<InvoiceServiceRelation> InvoiceServiceRelation()
        {
            return Db.InvoiceServiceRelation; 
        }

        public Invoice GetWithInclude(long invoiceId)
        {
            return Db.Invoice.Include(x => x.Services).FirstOrDefault(x => x.Id == invoiceId);
        }

        public void RemoveInvoiceServices(long id)
        {
            var services = Db.InvoiceServiceRelation.Where(x => x.InvoiceId == id).ToList();
            Db.RemoveRange(services);
            Db.SaveChanges();
        }

    }
}
