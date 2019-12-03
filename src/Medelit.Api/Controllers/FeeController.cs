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
    public class FeeController : ApiController
    {
        private readonly IFeeService _feeService;
        private readonly INotificationHandler<DomainNotification> _notifications;
        private readonly ILogger<FeeController> _logger;

        public FeeController(
            IFeeService feeService,
            INotificationHandler<DomainNotification> notifications,
            ILogger<FeeController> logger,
            IMediatorHandler mediator) : base(notifications, mediator)
        {
            _feeService = feeService;
            _notifications = notifications;
            _logger = logger;
        }

        [HttpGet("fees")]
        public IActionResult GetFees()
        {
         
            return Response(_feeService.GetFees());
        }

        [HttpPost("fees/find")]
        public IActionResult FindFees([FromBody] SearchViewModel model)
        {

            return Response(_feeService.FindFees(model));
        }

    }
}