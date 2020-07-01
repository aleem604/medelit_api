using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Medelit.Application;
using Medelit.Domain.Core.Bus;
using Medelit.Domain.Core.Notifications;
using Medelit.Common;
using System.Collections.Generic;

namespace Medelit.Api.Controllers
{
    public class InvoiceController : ApiController
    {
        private readonly IInvoiceService _invoiceService;
        private readonly INotificationHandler<DomainNotification> _notifications;
        private readonly ILogger<InvoiceController> _logger;

        public InvoiceController(
            IInvoiceService invoiceService,
            INotificationHandler<DomainNotification> notifications,
            ILogger<InvoiceController> logger,
            IMediatorHandler mediator) : base(notifications, mediator)
        {
            _invoiceService = invoiceService;
            _notifications = notifications;
            _logger = logger;
        }

        [HttpGet("invoices")]
        public IActionResult GetInvoices()
        {
            return Response(_invoiceService.GetInvoices());
        }

        [HttpPost("invoices/find")]
        public IActionResult FindInvoices([FromBody] SearchViewModel model)
        {
            _invoiceService.FindInvoices(model);
            return Response();
        }

        [HttpGet("invoices/{invoiceId}")]
        public IActionResult GetInvoiceById(long invoiceId)
        {
            _invoiceService.GetInvoiceById(invoiceId);
            return Response();
        }

        [HttpGet("invoices/invoice-bookings-for-crud/{invoiceId}")]
        public IActionResult InvocieBookingsForCrud(long invoiceId)
        {
            _invoiceService.InvocieBookingsForCrud(invoiceId);
            return Response();
        }

        [HttpPost("invoices/invoice-bookings-for-crud/{invoiceId}")]
        public IActionResult SaveInvocieBookingsForCrud([FromBody]IEnumerable<FilterModel> model, long invoiceId)
        {
            _invoiceService.SaveInvocieBookingsForCrud(model, invoiceId);
            return Response();
        }



        [HttpPost("invoices")]
        [HttpPut("invoices")]
        public IActionResult SaveInvoice([FromBody] InvoiceViewModel model)
        {
            _invoiceService.SaveInvoice(model);
            return Response();
        }

        [HttpPut("invoices/update-status/{status}")]
        public IActionResult UpdateStatus([FromBody] IEnumerable<InvoiceViewModel> invoices, eRecordStatus status)
        {
            _invoiceService.UpdateStatus(invoices, status);
            return Response();
        }

        //api/v1/
        [HttpDelete("invoices/{invoiceId}")]
        public IActionResult DeleteInvoice(long invoiceId)
        {
            _invoiceService.DeleteInvoices(new List<long> { invoiceId });
            return Response();
        }

        //api/v1/
        [HttpPut("invoices/delete")]
        public IActionResult DeleteInvoices([FromBody] IEnumerable<long> invoiceIds)
        {
            _invoiceService.DeleteInvoices(invoiceIds);
            return Response();
        }

        //api/v1/
        [HttpGet("invoices/bookings-to-add-to-invoice/{invoiceId}")]
        public IActionResult GetBookingToAddToInvoice(long invoiceId)
        {
            _invoiceService.GetBookingToAddToInvoice(invoiceId);
            return Response();
        }

        //api/v1/
        [HttpGet("invoices/process-invoice-emission/{invoiceId}")]
        public IActionResult ProcessInvocieEmissional(long invoiceId)
        {
            _invoiceService.ProcessInvoiceEmission(invoiceId);
            return Response();
        }

        //api/v1/
        [HttpPost("invoices/add-bookings-to-invoice/{invoiceId}")]
        public IActionResult AddBookingsToInvoice([FromBody] IEnumerable<long> bookingIds, long invoiceId)
        {
            _invoiceService.AddBookingsToInvoice(bookingIds, invoiceId);
            return Response();
        }


        //api/v1/
        [HttpPost("invoices/add-booking-to-invoice/{bookingId}")]
        [HttpPost("invoices/add-booking-to-invoice/{bookingId}/{invoiceId}")]
        public IActionResult AddBookingToInvoice(long bookingId, long invoiceId)
        {
            _invoiceService.AddBookingToInvoice(bookingId, invoiceId);
            return Response();
        }

        //api/v1/
        [HttpDelete("invoices/delete-invoice-booking/{invoiceid}/{bookingId}")]
        public IActionResult DeleteInvoiceBooking(long invoiceId, long bookingId)
        {
            _invoiceService.DeleteInvoiceBooking(invoiceId, bookingId);
            return Response();
        }

        //api/v1/
        [HttpGet("invoices/view/{invoiceId}")]
        public IActionResult GetInvoiceView(long invoiceId)
        {
            return Response(_invoiceService.GetInvoiceView(invoiceId));
        }

        [HttpGet("invoices/invoice-connected-professionals/{invoiceId}")]
        public IActionResult GetBookingConnectedProfessionals(long invoiceId)
        {
            return Response(_invoiceService.InvoiceConnectedProfessional(invoiceId));
        }

        [HttpGet("invoices/invoice-connected-customers/{invoiceId}")]
        public IActionResult GetBookingConnectedCustomers(long invoiceId)
        {
            return Response(_invoiceService.InvoiceConnectedCustomers(invoiceId));
        }

        [HttpGet("invoices/invoice-connected-invoice-entities/{invoiceId}")]
        public IActionResult InvoiceConnectedInvoiceEntity(long invoiceId)
        {
            return Response(_invoiceService.InvoiceConnectedInvoiceEntity(invoiceId));
        }

        [HttpGet("invoices/invoice-connected-bookings/{invoiceId}")]
        public IActionResult InvoiceConnectedInvoiceBooking(long invoiceId)
        {
            return Response(_invoiceService.InvoiceConnectedBookings(invoiceId));
        }

        

    }
}