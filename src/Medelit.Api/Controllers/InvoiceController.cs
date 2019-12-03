using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Medelit.Application;
using Medelit.Domain.Core.Bus;
using Medelit.Domain.Core.Notifications;
using Medelit.Common;

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

    }
}