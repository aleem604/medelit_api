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
        void GetInvoiceById(long invoiceId);
        void SaveInvoice(InvoiceViewModel model);
        void UpdateStatus(IEnumerable<InvoiceViewModel> invoices, eRecordStatus status);
        void DeleteInvoices(IEnumerable<long> invoiceIds);
        void ProcessInvoiceEmission(long invoiceId);
        void GetBookingToAddToInvoice(long invoiceId);
        void AddBookingsToInvoice(IEnumerable<long> bookingIds, long invoiceId);
        void AddBookingToInvoice(long bookingId, long invoiceId);
        void DeleteInvoiceBooking(long ibid);
        dynamic GetInvoiceView(long invoiceId);
        dynamic InvoiceConnectedProfessional(long bookingId);
        dynamic InvoiceConnectedCustomers(long invoiceId);
        dynamic InvoiceConnectedInvoiceEntity(long invoiceId);
        dynamic InvoiceConnectedBookings(long invoiceId);
        
    }
}
