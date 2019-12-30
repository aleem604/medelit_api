﻿using MediatR;
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
            return Response(_invoiceService.FindInvoices(model));
        }

        [HttpGet("invoices/{invoiceId}")]
        public IActionResult GetInvoiceById(long invoiceId)
        {

            return Response(_invoiceService.GetInvoiceById(invoiceId));
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
        [HttpPost("invoices/add-booking-to-invoice/{bookingId}")]
        [HttpPost("invoices/add-booking-to-invoice/{bookingId}/{invoiceId}")]
        public IActionResult AddBookingToInvoice(long bookingId, long invoiceId)
        {
            _invoiceService.AddBookingToInvoice(bookingId, invoiceId);
            return Response();
        }

        //api/v1/
        [HttpDelete("invoices/delete-invoice-booking/{ibid}")]
        public IActionResult DeleteInvoiceBooking(long ibid)
        {
            _invoiceService.DeleteInvoiceBooking(ibid);
            return Response();
        }

        //api/v1/
        [HttpGet("invoices/view/{invoiceId}")]
        public IActionResult GetInvoiceView(long invoiceId)
        {
            return Response(_invoiceService.GetInvoiceView(invoiceId));
        }

    }
}