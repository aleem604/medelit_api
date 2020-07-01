using Medelit.Common;
using Medelit.Domain.Models;

namespace Medelit.Domain.Interfaces
{
    public interface IInvoiceEntityRepository : IRepository<InvoiceEntity>
    {
        void FindInvoiceEntities(SearchViewModel viewModel);
        dynamic InvoiceEntityConnectedServices(long invoiceEntityId);
        dynamic InvoiceEntityConnectedInvoices(long invoiceEntityId);
        dynamic InvoiceEntityConnectedProfessionals(long invoiceEntityId);
        dynamic InvoiceEntityConnectedCustomers(long invoiceEntityId);
        dynamic InvoiceEntityConnectedBookings(long invoiceEntityId);
        dynamic InvoiceEntityConnectedLeads(long invoiceEntityId);
    }
}