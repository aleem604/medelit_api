using Medelit.Common;
using Medelit.Domain.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Medelit.Domain.Interfaces
{
    public interface IInvoiceRepository : IRepository<Invoice>
    {
        void FindInvoices(SearchViewModel viewModel);
        void InvocieBookingsForCrud(long invoiceId);
        InvoiceBookings AddBookingToInvoice(long bookingId, long invoiceId);
        void SaveInvocieBookingsForCrud(IEnumerable<FilterModel> model, long invoiceId);
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
        string GetProformaInoviceNumber(long invoiceId);
        (string, string) GetInvoiceHtml(long invoiceId);
    }
}