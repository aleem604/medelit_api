using System.Collections.Generic;
using Medelit.Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace Medelit.Domain.Interfaces
{
    public interface IBookingRepository : IRepository<Booking>
    {
        string GetBookingName(long customerId, string name, string surName);
        int GetSrNo(long customerId);
        dynamic GetBookingCycleConnectedBookings(long bookingId);
        dynamic BookingConnectedProfessional(long bookingId);
        dynamic BookingConnectedInvoices(long bookingId);
        string GetBookingInvoiceNumber(long invoiceId);
    }
}