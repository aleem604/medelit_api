using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Medelit.Common;

namespace Medelit.Application
{
    public interface IInvoiceService : IDisposable
    {
        dynamic GetInvoices();
        dynamic FindInvoices(SearchViewModel model);
        InvoiceViewModel GetInvoiceById(long invoiceId);
        void SaveInvoice(InvoiceViewModel model);
        void UpdateStatus(IEnumerable<InvoiceViewModel> invoices, eRecordStatus status);
        void DeleteInvoices(IEnumerable<long> invoiceIds);
        void AddBookingToInvoice(long bookingId, long invoiceId);
        void DeleteInvoiceBooking(long ibid);
        dynamic GetInvoiceView(long invoiceId);
    }
}
