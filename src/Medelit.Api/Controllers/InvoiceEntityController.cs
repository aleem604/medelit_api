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
            _invoiceEntityService.FindInvoiceEntities(model);
            return Response();
        }

        [HttpGet("invoice-entities/{ieId}")]
        public IActionResult GetLeadById(long ieId)
        {

            return Response(_invoiceEntityService.GetInvoiceEntityById(ieId));
        }

        [HttpPost("invoice-entities")]
        [HttpPut("invoice-entities")]
        public IActionResult SaveProvessional([FromBody] InvoiceEntityViewModel model)
        {
            _invoiceEntityService.SaveInvoiceEntity(model);
            return Response();
        }

        [HttpPut("invoice-entities/update-status/{status}")]
        public IActionResult UpdateStatus([FromBody] IEnumerable<InvoiceEntityViewModel> invoiceEntities, eRecordStatus status)
        {
            _invoiceEntityService.UpdateStatus(invoiceEntities, status);
            return Response();
        }

        //api/v1/
        [HttpDelete("invoice-entities/{entityId}")]
        public IActionResult DeleteLead(long entityId)
        {
            _invoiceEntityService.DeleteInvoiceEntities(new List<long> { entityId });
            return Response();
        }

        //api/v1/
        [HttpPut("invoice-entities/delete")]
        public IActionResult DeleteLeads([FromBody] IEnumerable<long> ids)
        {
            _invoiceEntityService.DeleteInvoiceEntities(ids);
            return Response();
        }

        //api/v1/
        [HttpGet("invoice-entities/invoice-entity-connected-services/{ieId}")]
        public IActionResult InvoiceEntityConnectedServices(long ieId)
        {
            return Response(_invoiceEntityService.InvoiceEntityConnectedServices(ieId));
        }

        //api/v1/
        [HttpGet("invoice-entities/invoice-entity-connected-invoices/{ieId}")]
        public IActionResult InvoiceEntityConnectedInvoices(long ieId)
        {
            return Response(_invoiceEntityService.InvoiceEntityConnectedInvoices(ieId));
        }

        //api/v1/
        [HttpGet("invoice-entities/invoice-entity-connected-professionals/{ieId}")]
        public IActionResult InvoiceEntityConnectedProfessionals(long ieId)
        {
            return Response(_invoiceEntityService.InvoiceEntityConnectedProfessionals(ieId));
        }

        //api/v1/
        [HttpGet("invoice-entities/invoice-entity-connected-customers/{ieId}")]
        public IActionResult InvoiceEntityConnectedCustomers(long ieId)
        {
            return Response(_invoiceEntityService.InvoiceEntityConnectedCustomers(ieId));
        }

        //api/v1/
        [HttpGet("invoice-entities/invoice-entity-connected-bookings/{ieId}")]
        public IActionResult InvoiceEntityConnectedBookings(long ieId)
        {
            return Response(_invoiceEntityService.InvoiceEntityConnectedBookings(ieId));
        }

        //api/v1/
        [HttpGet("invoice-entities/invoice-entity-connected-leads/{ieId}")]
        public IActionResult InvoiceEntityConnectedLeads(long ieId)
        {
            return Response(_invoiceEntityService.InvoiceEntityConnectedLeads(ieId));
        }

    }
}