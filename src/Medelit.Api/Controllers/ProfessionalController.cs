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
        public IActionResult SaveProvessional([FromBody] ProfessionalRequestViewModel model)
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
        public IActionResult UpdateStatus([FromBody] IList<ProfessionalRequestViewModel> fees, eRecordStatus status)
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

    }
}