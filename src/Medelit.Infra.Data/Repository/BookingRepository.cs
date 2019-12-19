using System.Collections.Generic;
using System.Linq;
using Medelit.Domain.Interfaces;
using Medelit.Domain.Models;
using Medelit.Infra.Data.Context;
using Medelit.Infra.Data.Repository;
using Microsoft.EntityFrameworkCore;

namespace Equinox.Infra.Data.Repository
{
    public class BookingRepository : Repository<Booking>, IBookingRepository
    {
        public BookingRepository(MedelitContext context)
            : base(context)
        {}

       

        public Booking GetWithInclude(long bookingId)
        {
            return Db.Booking.Include(x => x.Services).FirstOrDefault(x => x.Id == bookingId);
        }

        public void RemoveBookingServices(long bookingId)
        {
            var services = Db.BookingServiceRelation.Where(x => x.BookingId == bookingId).ToList();
            Db.BookingServiceRelation.RemoveRange(services);
            Db.SaveChanges();
        }

        public void SaveBookingRelation(List<BookingServiceRelation> newServices)
        {
            Db.BookingServiceRelation.AddRange(newServices);
            Db.SaveChanges();
        }

        public void SaveInvoiceRelation(List<InvoiceServiceRelation> newServices, Invoice invoice)
        {
            Db.Invoice.Update(invoice);
            Db.InvoiceServiceRelation.AddRange(newServices);
            Db.SaveChanges();
        }

        public string GetBookingName(string name, string surName)
        {
            name = $"{name} {surName}";
            var bookingCount = Db.Booking.Where(x => x.Name.StartsWith(name)).Count();

            return $"{name} {bookingCount ++}";
        }


    }
}
