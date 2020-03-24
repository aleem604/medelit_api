using Medelit.Common;
using Medelit.Domain.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;

namespace Medelit.Domain.Interfaces
{
    public interface IInvoiceRepository : IRepository<Invoice>
    {
        InvoiceBookings AddBookingToInvoice(long bookingId, long invoiceId);
        IQueryable<InvoiceBookings> GetInvoiceBookings();
        void UpdateInvoiceTotal(long invoiceId);
        void UpdateBookingStats(List<long> bookingIds);
        void DeleteInvoiceBooking(InvoiceBookings invoiceBooking);
        void ProcessInvoiceEmission(long invoiceId);
        void GetInvoiceById(long invoiceId, IEnumerable<AuthUser> enumerable);
        void GetBookingToAddToInvoice(long invoiceId);
        void AddBookingsToInvoice(IEnumerable<long> bookingIds, long invoiceId);
        dynamic GetInvoiceView(long invoiceId);
        dynamic InvoiceConnectedProfessional(long invoiceId);
        dynamic InvoiceConnectedCustomers(long invoiceId);
        dynamic InvoiceConnectedInvoiceEntity(long invoiceId);
        dynamic InvoiceConnectedBookings(long invoiceId);
       
    }
}