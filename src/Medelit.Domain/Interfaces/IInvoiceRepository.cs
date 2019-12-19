using Medelit.Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace Medelit.Domain.Interfaces
{
    public interface IInvoiceRepository : IRepository<Invoice>
    {
        DbSet<InvoiceServiceRelation> InvoiceServiceRelation();
        Invoice GetWithInclude(long invoiceId);
        void RemoveInvoiceServices(long id);
    }
}