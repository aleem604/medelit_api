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

        [HttpGet("professionals")]
        public IActionResult GetProfessionals()
        {
         
            return Response(_professionalService.GetProfessionals());
        }

        [HttpPost("professionals/find")]
        public IActionResult FindProfessionals([FromBody] SearchViewModel model)
        {

            return Response(_professionalService.FindProfessionals(model));
        }

    }
}