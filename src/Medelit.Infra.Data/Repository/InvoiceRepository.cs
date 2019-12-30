using System.Linq;
using Medelit.Domain.Interfaces;
using Medelit.Domain.Models;
using Medelit.Infra.Data.Context;
using Medelit.Infra.Data.Repository;
using Microsoft.EntityFrameworkCore;

namespace Medelit.Infra.Data.Repository
{
    public class InvoiceRepository : Repository<Invoice>, IInvoiceRepository
    {
        public InvoiceRepository(MedelitContext context)
            : base(context)
        {
        }

        public IQueryable<InvoiceBookings> GetInvoiceBookings()
        {
            return Db.InvoiceBookings;
        }

        public InvoiceBookings AddBookingToInvoice(long bookingId, long invoiceId)
        {
            var invioceBooking = new InvoiceBookings { BookingId = bookingId, InvoiceId = invoiceId };
            Db.InvoiceBookings.Add(invioceBooking);
            Db.SaveChanges();
            return invioceBooking;

        }

        public void DeleteInvoiceBooking(InvoiceBookings ib)
        {
            Db.InvoiceBookings.Remove(ib);
        }

        public dynamic GetInvoiceView(long invoiceId)
        {
            var invoiceInfo = Db.Invoice.Include(x=>x.Customer).FirstOrDefault(x=>x.Id == invoiceId);
            var paymentMethods = Db.StaticData.Select((s) => new { s.Id, Value = s.PaymentMethods }).Where(x => x.Value != null);
            var status = Db.StaticData.Select((s) => new { s.Id, Value = s.PaymentStatus }).Where(x => x.Value != null);
            
            var invoiceBookings = Db.InvoiceBookings.Where(x => x.InvoiceId == invoiceId).ToList();
            var bookingIds = invoiceBookings.Select(b => b.BookingId).ToList();
            var bookings = Db.Booking.Where(x => bookingIds.Contains(x.Id)).Include(x=>x.Service).ToList();
            
            return new {
                invoiceInfo.Id,
                invoiceInfo.Subject,
                invoiceInfo.PaymentMethodId,
                invoiceInfo.InvoiceNumber,
                PaymentMethod = paymentMethods.FirstOrDefault(x=>x.Id == invoiceInfo.PaymentMethodId)?.Value,
                invoiceInfo.StatusId,
                Status = paymentMethods.FirstOrDefault(x=>x.Id == invoiceInfo.StatusId)?.Value,
                invoiceInfo.DueDate,
                invoiceInfo.PaymentDue,
                invoiceInfo.TotalInvoice,

                invoiceInfo.Customer.BankName,
                invoiceInfo.Customer.SortCode,
                invoiceInfo.Customer.AccountNumber,
                invoiceInfo.CustomerId,
                invoiceInfo.Customer.SurName,
                invoiceInfo.Customer.Name,
                invoiceInfo.Customer.HomePostCode,
                invoiceInfo.Customer.HomeCityId,
                City = Db.City.FirstOrDefault(x=> x.Id == invoiceInfo.Customer.HomeCityId)?.Value,
                Country = Db.Countries.FirstOrDefault(x=> x.Id == invoiceInfo.Customer.HomeCountryId)?.Value,


                bookings,
            };
        }


    }
}
