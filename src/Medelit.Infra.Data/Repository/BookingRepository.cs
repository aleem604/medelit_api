using System.Collections.Generic;
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
    public class BookingRepository : Repository<Booking>, IBookingRepository
    {

        public BookingRepository(MedelitContext context, IHttpContextAccessor contextAccessor, IMediatorHandler bus)
            : base(context, contextAccessor, bus)
        { }

        public string GetBookingInvoiceNumber(long invoiceId)
        {
            var obj = Db.Invoice.FirstOrDefault(x => x.Id == invoiceId);
            if (obj is null)
                return string.Empty;
            return obj.InvoiceNumber;
        }

        public string GetBookingName(long customerId, string name, string surName)
        {
            return $"{name} {surName}";
        }

        public int GetSrNo(long customerId)
        {
            var obj = Db.Booking.Where(x => x.CustomerId == customerId).Select(x=> new {x.SrNo }).ToList();
            if (obj is null || obj.Count == 0 )
                return 1;
            else
                return obj.Max(x => x.SrNo).Value + 1;
        }

        public dynamic GetBookingCycleConnectedBookings(long bookingId)
        {
            var cycleBooking = Db.Booking.FirstOrDefault(x => x.Id == bookingId).CycleBookingId;
            return (from b in Db.Booking
                    where b.CycleBookingId == cycleBooking && b.Id != bookingId && b.Cycle.HasValue
                    select new
                    {
                        bookingName = b.Name,
                        serviceName = b.Service.Name,
                        professional = b.Professional.Name,
                        cycleNumber = b.CycleNumber,
                        ptFee = b.PtFee,
                        proFee = b.ProFee,
                        b.SubTotal
                    }).ToList();
        }

        public dynamic BookingConnectedProfessional(long bookingId)
        {
            var collaborations = Db.StaticData.Select((s) => new { s.Id, Value = s.CollaborationCodes }).Where(x => x.Value != null).ToList();

            return (from b in Db.Booking
                    where b.Id == bookingId
                    select new
                    {
                        professional = b.Professional.Name,
                        phoneNumber = b.Professional.Telephone,
                        email = b.Professional.Email,
                        lastVisitDate = b.VisitStartDate,
                        Status = b.Professional.ActiveCollaborationId > 0 ? collaborations.FirstOrDefault(x => x.Id == b.Professional.ActiveCollaborationId).Value : string.Empty
                    }).ToList();
        }

        public dynamic BookingConnectedInvoices(long bookingId)
        {
            return (from ib in Db.InvoiceBookings
                    where ib.BookingId == bookingId
                    select new
                    {
                        subject = ib.Invoice.Subject,
                        invoiceNumber = ib.Invoice.InvoiceNumber,
                        ieName = ib.Booking.InvoiceEntityId.HasValue ? ib.Booking.InvoiceEntity.Name : string.Empty,
                        invoiceDate = ib.Invoice.InvoiceDate,
                        invoiceDueDate = ib.Invoice.DueDate,
                        totalInvoice = ib.Invoice.TotalInvoice
                    }).ToList();

        }

       public string GetItemNameOnInvoice(long serviceId, long? ptFeeId)
        {
            string serviceName = Db.Service.Where(x => x.Id == serviceId).Select(s => s.Name).FirstOrDefault();
            string feeName = ptFeeId.HasValue ? Db.PtFee.Where(x => x.Id == ptFeeId).Select(s => s.FeeName).FirstOrDefault() : string.Empty;
            return $"{serviceName} {feeName}";
        }

    }
}
