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
    public class InvoiceEntityController : ApiController
    {
        private readonly IInvoiceEntityService _invoiceEntityService;
        private readonly INotificationHandler<DomainNotification> _notifications;
        private readonly ILogger<InvoiceEntityController> _logger;

        public InvoiceEntityController(
            IInvoiceEntityService invoiceEntityService,
            INotificationHandler<DomainNotification> notifications,
            ILogger<InvoiceEntityController> logger,
            IMediatorHandler mediator) : base(notifications, mediator)
        {
            _invoiceEntityService = invoiceEntityService;
            _notifications = notifications;
            _logger = logger;
        }

        [HttpGet("invoice-entities")]
        public IActionResult GetInvoiceEntities()
        {
         
            return Response(_invoiceEntityService.GetInvoiceEntities());
        }

        [HttpPost("invoice-entities/find")]
        public IActionResult FindInvoiceEntities([FromBody] SearchViewModel model)
        {

            return Response(_invoiceEntityService.FindInvoiceEntities(model));
        }

    }
}