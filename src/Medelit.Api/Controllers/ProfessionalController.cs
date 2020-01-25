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
    public class ProfessionalController : ApiController
    {
        private readonly IProfessionalService _professionalService;
        private readonly INotificationHandler<DomainNotification> _notifications;
        private readonly ILogger<ProfessionalController> _logger;

        public ProfessionalController(
            IProfessionalService professionalService,
            INotificationHandler<DomainNotification> notifications,
            ILogger<ProfessionalController> logger,
            IMediatorHandler mediator) : base(notifications, mediator)
        {
            _professionalService = professionalService;
            _notifications = notifications;
            _logger = logger;
        }

        [HttpPost("professionals/find")]
        public IActionResult FindProfessionals([FromBody] SearchViewModel model)
        {
            return Response(_professionalService.FindProfessionals(model));
        }

        [HttpPost("professionals")]
        [HttpPut("professionals")]
        public IActionResult SaveProvessional([FromBody] ProfessionalViewModel model)
        {
            _professionalService.SaveProvessional(model);
            return Response();
        }

        [HttpGet("professionals/{professionalId}")]
        public IActionResult GetProfessionalById(long professionalId)
        {
            return Response(_professionalService.GetProfessionalById(professionalId));
        }

        [HttpPut("professionals/update-status/{status}")]
        public IActionResult UpdateStatus([FromBody] IList<ProfessionalViewModel> fees, eRecordStatus status)
        {
            _professionalService.UpdateStatus(fees, status);
            return Response();
        }
        //api/v1/
        [HttpPut("professionals/delete")]
        public IActionResult DeleteAttractions([FromBody] IEnumerable<long> feeIds)
        {
            _professionalService.DeleteFees(feeIds);
            return Response();
        }

        

        [HttpGet("professionals/connected-customers/{proId}")]
        public IActionResult GetConnectedCustomers(long proId)
        {
            return Response(_professionalService.GetConnectedCustomers(proId));
        }

        [HttpGet("professionals/connected-bookings/{proId}")]
        public IActionResult GetConnectedBookings(long proId)
        {
            return Response(_professionalService.GetConnectedBookings(proId));
        }

        [HttpGet("professionals/connected-invoices/{proId}")]
        public IActionResult GetConnectedInvoices(long proId)
        {
            return Response(_professionalService.GetConnectedInvoices(proId));
        }

        [HttpGet("professionals/connected-leads/{proId}")]
        public IActionResult GetConnectedLeads(long proId)
        {
            return Response(_professionalService.GetConnectedLeads(proId));
        }


        [HttpGet("professionals/professional-connected-services/{proId}")]
        public IActionResult GetProfessionalConnectedServices(long proId)
        {
            return Response(_professionalService.GetProfessionalConnectedServices(proId));
        }

        [HttpPost("professionals/detach-professional-connected-service/{proId}")]
        public IActionResult DetachProfessionalConnectedService([FromBody]IEnumerable<long> serviceIds, long proId)
        {
            return Response(_professionalService.DetachProfessionalConnectedService(serviceIds, proId));
        }

    }
}