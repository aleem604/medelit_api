using System.Collections.Generic;
using Medelit.Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace Medelit.Domain.Interfaces
{
    public interface IBookingRepository : IRepository<Booking>
    {
        Booking GetWithInclude(long bookingId);
        void RemoveBookingServices(long bookingId);
        void SaveBookingRelation(List<BookingServiceRelation> newServices);
        void SaveInvoiceRelation(List<InvoiceServiceRelation> newServices, Invoice invoice);
        string GetBookingName(string name, string surName);
    }
}