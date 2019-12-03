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
    public class FieldController : ApiController
    {
        private readonly IFieldSubcategoryService _fieldService;
        private readonly INotificationHandler<DomainNotification> _notifications;
        private readonly ILogger<FieldController> _logger;

        public FieldController(
            IFieldSubcategoryService fieldService,
            INotificationHandler<DomainNotification> notifications,
            ILogger<FieldController> logger,
            IMediatorHandler mediator) : base(notifications, mediator)
        {
            _fieldService = fieldService;
            _notifications = notifications;
            _logger = logger;
        }

        [HttpGet("fields")]
        public IActionResult GetLeads()
        {
         
            return Response(_fieldService.GetFields());
        }

        [HttpPost("fields/find")]
        public IActionResult FindFields([FromBody] SearchViewModel model)
        {

            return Response(_fieldService.FindFields(model));
        }

    }
}