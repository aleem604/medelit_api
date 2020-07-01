using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Medelit.Common;

namespace Medelit.Application
{
    public interface IInvoiceEntityService : IDisposable
    {
        void FindInvoiceEntities(SearchViewModel model);
        dynamic GetInvoiceEntities();
        InvoiceEntityViewModel GetInvoiceEntityById(long leadId);
        void SaveInvoiceEntity(InvoiceEntityViewModel model);
        void UpdateStatus(IEnumerable<InvoiceEntityViewModel> leads, eRecordStatus status);
        void DeleteInvoiceEntities(IEnumerable<long> entityIds);
        dynamic InvoiceEntityConnectedServices(long invoiceEntityId);
        dynamic InvoiceEntityConnectedInvoices(long invoiceEntityId);
        dynamic InvoiceEntityConnectedProfessionals(long invoiceEntityId);
        dynamic InvoiceEntityConnectedCustomers(long invoiceEntityId);
        dynamic InvoiceEntityConnectedBookings(long invoiceEntityId);
        dynamic InvoiceEntityConnectedLeads(long invoiceEntityId);
    }
}
