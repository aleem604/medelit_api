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
    public class LeadController : ApiController
    {
        private readonly ILeadService _leadService;
        private readonly INotificationHandler<DomainNotification> _notifications;
        private readonly ILogger<LeadController> _logger;

        public LeadController(
            ILeadService leadService,
            INotificationHandler<DomainNotification> notifications,
            ILogger<LeadController> logger,
            IMediatorHandler mediator) : base(notifications, mediator)
        {
            _leadService = leadService;
            _notifications = notifications;
            _logger = logger;
        }

        

        [HttpPost("leads/find")]
        public IActionResult FindLeads([FromBody] SearchViewModel model)
        {

            return Response(_leadService.FindLeads(model));
        }

        [HttpGet("leads")]
        public IActionResult GetLeads()
        {

            return Response(_leadService.GetLeads());
        }

        [HttpGet("leads/{leadId}")]
        public IActionResult GetLeadById(long leadId)
        {

            return Response(_leadService.GetLeadById(leadId));
        }

    }
}