using System;
using System.Collections.Generic;
using System.Linq;
using Medelit.Common;
using Medelit.Domain.Core.Bus;
using Medelit.Domain.Core.Notifications;
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
            UpdateInvoiceTotal(invoiceId);
            return invioceBooking;
        }

        public void DeleteInvoiceBooking(InvoiceBookings ib)
        {
            var booking = Db.Booking.Find(ib.InvoiceId);
            booking.InvoiceId = null;
            booking.UpdateDate = DateTime.UtcNow;
            booking.UpdatedById = CurrentUser.Id;
            Db.Booking.Update(booking);
            Db.InvoiceBookings.Remove(ib);
            Db.SaveChanges();
            UpdateInvoiceTotal(ib.InvoiceId);
        }

        public void ProcessInvoiceEmission(long invoiceId)
        {
            var invoice = Db.Invoice.Find(invoiceId);
            invoice.InvoiceNumber = $"{invoice.Id.ToString().PadLeft(5, '0')}/{DateTime.Now.ToString("ddMM/yyyy")}";
            invoice.IsProforma = false;
            Db.Invoice.Update(invoice);
            Db.SaveChanges();
        }

        public void GetInvoiceById(long invoiceId, IEnumerable<AuthUser> users)
        {
            try
            {
                var invoice = Db.Invoice.Find(invoiceId);
                invoice.AssignedTo = users.FirstOrDefault(x => x.Id == invoice.AssignedToId).FullName;
                var invoiceBookings = Db.InvoiceBookings.Where(x => x.InvoiceId == invoiceId).Select(x => x.Id).ToList();

                invoice.InvoiceBookingView = (from ib in Db.InvoiceBookings
                                              where ib.InvoiceId == invoiceId
                                              select new
                                              {
                                                  ib.Id,
                                                  ib.InvoiceId,
                                                  ib.BookingId,
                                                  Booking = new
                                                  {
                                                      ib.Booking.Id,
                                                      ib.Booking.Name,
                                                      ib.Booking.CustomerId,
                                                      CustomerName = ib.Booking.Customer.Name,
                                                      Quantity = ib.Booking.QuantityHours,
                                                      Service = ib.Booking.Service.Name,
                                                      ib.Booking.PtFees.FeeName,
                                                      ib.Booking.TaxAmount,
                                                      ib.Booking.PatientDiscount,
                                                      subTotal = ib.Booking.SubTotal,
                                                      ib.Booking.GrossTotal
                                                  }
                                              }).ToList();
                HandleResponse(GetType(), invoice);

            }
            catch (Exception ex)
            {
                HandleException(GetType(), ex);
            }
        }

        public void GetBookingToAddToInvoice(long invoiceId)
        {
            try
            {
                var invoiceBookings = (from bi in Db.InvoiceBookings where bi.InvoiceId == invoiceId select bi.BookingId).Distinct().ToList();
                var result = new List<dynamic>();

                var bookings = (from b in Db.Booking
                              where b.InvoiceId == null && b.IsValidated
                              select new
                              {
                                  b.Id,
                                  b.Name,
                                  b.PhoneNumber,
                                  b.PtFees.FeeName,
                                  b.InvoiceNumber,
                                  b.PaymentStatusId,
                                  b.PaymentConcludedId,
                                  b.PaymentMethodId,
                                  b.BookingStatusId
                              }).ToList();
                foreach (var b in bookings)
                {
                    var booking = new Booking {InvoiceNumber = b.InvoiceNumber, PaymentStatusId = b.PaymentStatusId, PaymentConcludedId = b.PaymentConcludedId, PaymentMethodId = b.PaymentMethodId };
                    if (booking.IsValid())
                        result.Add(b);
                }
              
                HandleResponse(GetType(), result);
            }
            catch (Exception ex)
            {
                HandleException(GetType(), ex);
            }
        }

        public void AddBookingsToInvoice(IEnumerable<long> bookingIds, long invoiceId)
        {
            try
            {
                foreach (var bookingId in bookingIds)
                {
                    var exists = Db.InvoiceBookings.FirstOrDefault(x => x.BookingId == bookingId && x.InvoiceId == invoiceId);
                    var booking = Db.Booking.Find(bookingId);
                    if (exists is null)
                    {
                        Db.InvoiceBookings.Add(new InvoiceBookings { InvoiceId = invoiceId, BookingId = bookingId });
                        booking.InvoiceId = invoiceId;
                        booking.UpdateDate = DateTime.UtcNow;
                        booking.UpdatedById = CurrentUser.Id;
                        Db.Booking.Update(booking);
                        Db.SaveChanges();
                    }
                    else
                    {
                        booking.InvoiceId = invoiceId;
                        booking.UpdateDate = DateTime.UtcNow;
                        booking.UpdatedById = CurrentUser.Id;
                        Db.Booking.Update(booking);
                        Db.SaveChanges();
                    }
                }
                UpdateInvoiceTotal(invoiceId);
                Db.SaveChanges();
                var invoiceBookings = (from ib in Db.InvoiceBookings
                                       join b in Db.Booking on ib.BookingId equals b.Id
                                       join c in Db.Customer on ib.Booking.CustomerId equals c.Id
                                       where ib.InvoiceId == invoiceId
                                       select new
                                       {
                                           ib.Id,
                                           ib.InvoiceId,
                                           ib.BookingId,
                                           Booking = new
                                           {
                                               b.Id,
                                               b.Name,
                                               b.CustomerId,
                                               CustomerName = c.Name,
                                               Service = b.Service.Name,
                                               b.PtFees.FeeName,
                                               b.TaxAmount,
                                               b.PatientDiscount,
                                               subTotal = b.SubTotal,
                                               b.GrossTotal
                                           }
                                       }).ToList();

                _bus.RaiseEvent(new DomainNotification(GetType().Name, null, invoiceBookings));

            }
            catch (Exception ex)
            {
                HandleException(GetType(), ex);
            }
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
                        proName = ib.Booking.Professional.Name,
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
                        service = $@"<span class='font-500'>Service:</span> {ib.Booking.Service.Name} <br/> <span class='font-500'>Gross Total:</span> {ib.Booking.GrossTotal}",
                        visitDate = ib.Booking.VisitStartDate,
                        professional = ib.Booking.Professional.Name
                    }).ToList();
        }

        public dynamic InvoiceConnectedInvoiceEntity(long invoiceId)
        {
            return (from ib in Db.Invoice
                    where ib.Id == invoiceId
                    select new
                    {
                        invoiceEntity = ib.InvoiceEntity.Name,
                        customer = ib.Customer.Name,
                        phoneNumber = ib.InvoiceEntity.MainPhoneNumber,
                        email = ib.InvoiceEntity.Email,
                        service = $@"<span class='font-500'>Service(s):</span> {string.Join(", ", ib.InvoiceBookings.Select(s=>s.Booking.Service.Name).Distinct().ToList())} <br/> <span class='font-500'>Gross Total:</span> {ib.InvoiceBookings.Select(s => s.Booking.GrossTotal).Sum()}",
                        visitDate = ib.DateOfVisit,
                        professional = string.Join(", ", ib.InvoiceBookings.Select(s => s.Booking.Professional.Name).ToList())
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

        public void UpdateInvoiceTotal(long invoiceId)
        {
            try
            {
                var invoice = Db.Invoice.Find(invoiceId);
                invoice.TotalInvoice = InvoiceTotals(invoiceId);
                invoice.UpdateDate = DateTime.UtcNow;
                invoice.UpdatedById = CurrentUser.Id;
                Db.Invoice.Update(invoice);
                Db.SaveChanges();
            }
            catch (Exception)
            {

            }
        }

        public decimal? InvoiceTotals(long invoiceId)
        {
            var bookingIds = Db.InvoiceBookings.Where(x => x.InvoiceId == invoiceId).Select(x => x.BookingId).Distinct().ToList();
            UpdateBookingStats(bookingIds);
            decimal? totals = 0;
            foreach (var bookingId in bookingIds)
            {
                var bookingGrossTotal = Db.Booking.Where(x => x.Id == bookingId).Select(x => x.GrossTotal).FirstOrDefault();
                if (bookingGrossTotal.HasValue)
                {
                    totals += bookingGrossTotal.Value;
                }
            }
            return totals;
        }

        public void UpdateBookingStats(List<long> bookingIds)
        {
            try
            {
                foreach (var bookingId in bookingIds)
                {
                    var bookingModel = Db.Booking.Find(bookingId);
                    if (bookingModel != null)
                    {
                        bookingModel.SubTotal = GetSubTotal(bookingModel.PtFee, bookingModel.QuantityHours);
                        bookingModel.TaxAmount = GetCusotmerTaxAmount(bookingModel.SubTotal, bookingModel.TaxType);

                        bookingModel.GrossTotal = (Convert.ToDecimal(bookingModel.SubTotal) + Convert.ToDecimal(bookingModel.TaxAmount)) - Convert.ToDecimal(bookingModel.PatientDiscount);

                        Db.Booking.Update(bookingModel);
                        Db.SaveChanges();
                    }
                }
            }
            catch
            {
            }
        }


    }
}
