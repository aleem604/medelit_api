using System.Linq;
using Medelit.Domain.Core.Bus;
using Medelit.Domain.Interfaces;
using Medelit.Domain.Models;
using Medelit.Infra.Data.Context;
using Medelit.Infra.Data.Repository;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace Medelit.Infra.Data.Repository
{
    public class InvoiceRepository : Repository<Invoice>, IInvoiceRepository
    {
        public InvoiceRepository(MedelitContext context, IHttpContextAccessor contextAccessor, IMediatorHandler bus)
            : base(context, contextAccessor, bus)
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
            var invoiceInfo = Db.Invoice.Include(x => x.Customer).FirstOrDefault(x => x.Id == invoiceId);
            var paymentMethods = Db.StaticData.Select((s) => new { s.Id, Value = s.PaymentMethods }).Where(x => x.Value != null);
            var status = Db.StaticData.Select((s) => new { s.Id, Value = s.PaymentStatus }).Where(x => x.Value != null);

            var invoiceBookings = Db.InvoiceBookings.Where(x => x.InvoiceId == invoiceId).ToList();
            var bookingIds = invoiceBookings.Select(b => b.BookingId).ToList();
            var bookings = Db.Booking.Where(x => bookingIds.Contains(x.Id)).Include(x => x.Service).ToList();

            return new
            {
                invoiceInfo.Id,
                invoiceInfo.Subject,
                invoiceInfo.PaymentMethodId,
                invoiceInfo.InvoiceNumber,
                PaymentMethod = paymentMethods.FirstOrDefault(x => x.Id == invoiceInfo.PaymentMethodId)?.Value,
                invoiceInfo.StatusId,
                Status = paymentMethods.FirstOrDefault(x => x.Id == invoiceInfo.StatusId)?.Value,
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
                City = Db.City.FirstOrDefault(x => x.Id == invoiceInfo.Customer.HomeCityId)?.Value,
                Country = Db.Countries.FirstOrDefault(x => x.Id == invoiceInfo.Customer.HomeCountryId)?.Value,


                bookings,
            };
        }

        public dynamic InvoiceConnectedProfessional(long invoiceId)
        {
            var collaborations = Db.StaticData.Select((s) => new { s.Id, Value = s.CollaborationCodes }).Where(x => x.Value != null).ToList();
            return (from ib in Db.InvoiceBookings
                    where ib.InvoiceId == invoiceId
                    select new
                    {
                        proName =ib.Booking.Professional.Name,
                        phone = ib.Booking.Professional.HomePhone,
                        ib.Booking.Professional.Email,
                        lastVisitDate = ib.Booking.VisitStartDate,
                        ib.Booking.Professional.ActiveCollaborationId,
                         Status = ib.Booking.Professional.ActiveCollaborationId > 0 ? collaborations.FirstOrDefault(x => x.Id == ib.Booking.Professional.ActiveCollaborationId).Value : string.Empty
                    }).ToList();
        }

        public dynamic InvoiceConnectedCustomers(long invoiceId)
        {
            return (from ib in Db.InvoiceBookings
                    where ib.InvoiceId == invoiceId
                    select new
                    {
                        customer = ib.Booking.Customer.Name,
                        phoneNumber = ib.Booking.Customer.MainPhone,
                        email = ib.Booking.Customer.Email,
                        service = $@"<span class='font-500'>Service:</span> {ib.Booking.Service.Name} <br/> <span class='font-500'>Gross Total:</strong> {ib.Booking.GrossTotal}",
                        visitDate = ib.Booking.VisitStartDate,
                        professional = ib.Booking.Professional.Name
                    }).ToList();
        }

        public dynamic InvoiceConnectedInvoiceEntity(long invoiceId)
        {
            return (from ib in Db.InvoiceBookings
                    where ib.Invoice.Id == invoiceId
                    select new
                    {
                        invoiceEntity = ib.Invoice.InvoiceEntity.Name,
                        customer = ib.Invoice.Customer.Name,
                        phoneNumber = ib.Invoice.InvoiceEntity.MainPhoneNumber,
                        email = ib.Invoice.InvoiceEntity.Email,
                        service = $@"<span class='font-500'>Service:</span> {ib.Booking.Service.Name} <br/> <span class='font-500'>Gross Total:</span> {ib.Booking.GrossTotal}",
                        visitDate = ib.Invoice.DateOfVisit,
                        professional = ib.Booking.Professional.Name
                    }).ToList();
        }

        public dynamic InvoiceConnectedBookings(long invoiceId)
        {
            var statuses = Db.StaticData.Select((s) => new { s.Id, Value = s.InvoiceStatus }).Where(x => x.Value != null).ToList();
            return (from ib in Db.InvoiceBookings
                    where ib.Invoice.Id == invoiceId
                    select new
                    {
                        bookingName = ib.Booking.Name,
                        Service = $@"Service: {ib.Booking.Service.Name} <br/> Gross Total: {ib.Booking.GrossTotal}",
                        professional = ib.Booking.Professional.Name,
                        proFee = (ib.Booking.ProFee.HasValue ? ib.Booking.ProFee.Value.ToString("G29") : string.Empty),
                        visitDate = ib.Booking.VisitStartDate,
                        ib.Invoice.StatusId,
                        Status = ib.Invoice.StatusId.HasValue ? statuses.FirstOrDefault(x => x.Id == ib.Invoice.StatusId).Value : string.Empty,
                    }).ToList();
        }

        

    }
}
