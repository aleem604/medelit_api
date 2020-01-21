using Medelit.Domain.Models;

namespace Medelit.Domain.Interfaces
{
    public interface IInvoiceEntityRepository : IRepository<InvoiceEntity>
    {
        dynamic InvoiceEntityConnectedServices(long invoiceEntityId);
        dynamic InvoiceEntityConnectedInvoices(long invoiceEntityId);
        dynamic InvoiceEntityConnectedProfessionals(long invoiceEntityId);
        dynamic InvoiceEntityConnectedCustomers(long invoiceEntityId);
        dynamic InvoiceEntityConnectedBookings(long invoiceEntityId);
        dynamic InvoiceEntityConnectedLeads(long invoiceEntityId);
    }
}