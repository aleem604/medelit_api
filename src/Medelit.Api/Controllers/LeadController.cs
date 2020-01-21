using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Medelit.Application;
using Medelit.Domain.Core.Bus;
using Medelit.Domain.Core.Notifications;
using Medelit.Common;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Medelit.Api.Controllers
{
    public class LeadController : ApiController
    {
        private readonly ILeadService _leadService;
        private readonly INotificationHandler<DomainNotification> _notifications;
        private readonly ILogger<LeadController> _logger;
        private readonly IMediatorHandler _bus;

        public LeadController(
            ILeadService leadService,
            INotificationHandler<DomainNotification> notifications,
            ILogger<LeadController> logger,
            IMediatorHandler bus) : base(notifications, bus)
        {
            _leadService = leadService;
            _notifications = notifications;
            _bus = bus;
            _logger = logger;
        }



        [HttpPost("leads/find")]
        public IActionResult FindLeads([FromBody] SearchViewModel model)
        {
            return Response(_leadService.FindLeads(model));
        }

        [Authorize]
        [HttpGet("leads")]
        public IActionResult GetLeads()
        {
            return Response(_leadService.GetLeads());
        }

        [HttpGet("leads/{leadId}")]
        [HttpGet("leads/{leadId}/{fromCustomerId}")]
        public IActionResult GetLeadById(long leadId, long? fromCustomerId)
        {

            return Response(_leadService.GetLeadById(leadId, fromCustomerId));
        }

        [HttpPost("leads")]
        [HttpPut("leads")]
        public IActionResult SaveProvessional([FromBody] LeadViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return Response(null, GetModelStateErrors());
            }
            else
                _leadService.SaveLead(model);
            return Response();
        }

        [HttpPut("leads/update-status/{status}")]
        public IActionResult UpdateStatus([FromBody] IEnumerable<LeadViewModel> leads, eRecordStatus status)
        {
            _leadService.UpdateStatus(leads, status);
            return Response();
        }

        //api/v1/
        [HttpDelete("leads/{leadId}")]
        public IActionResult DeleteLead(long leadId)
        {
            _leadService.DeleteLeads(new List<long> { leadId });
            return Response();
        }

        //api/v1/
        [HttpPut("leads/delete")]
        public IActionResult DeleteLeads([FromBody] IEnumerable<long> leadIds)
        {
            _leadService.DeleteLeads(leadIds);
            return Response();
        }

        [HttpGet("leads/convert-booking/{leadId}")]
        public IActionResult ConvertToBooking(long leadId)
        {
            _leadService.ConvertToBooking(leadId);
            return Response();
        }
    }
}