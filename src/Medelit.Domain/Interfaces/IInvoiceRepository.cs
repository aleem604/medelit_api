using Medelit.Domain.Models;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace Medelit.Domain.Interfaces
{
    public interface IInvoiceRepository : IRepository<Invoice>
    {
        InvoiceBookings AddBookingToInvoice(long bookingId, long invoiceId);
        IQueryable<InvoiceBookings> GetInvoiceBookings();
        void DeleteInvoiceBooking(InvoiceBookings invoiceBooking);
        dynamic GetInvoiceView(long invoiceId);
        dynamic InvoiceConnectedProfessional(long invoiceId);
        dynamic InvoiceConnectedCustomers(long invoiceId);
        dynamic InvoiceConnectedInvoiceEntity(long invoiceId);
        dynamic InvoiceConnectedBookings(long invoiceId);
    }
}