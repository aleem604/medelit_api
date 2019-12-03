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
    public class ServiceController : ApiController
    {
        private readonly IServiceService _serviceService;
        private readonly INotificationHandler<DomainNotification> _notifications;
        private readonly ILogger<ServiceController> _logger;

        public ServiceController(
            IServiceService serviceService,
            INotificationHandler<DomainNotification> notifications,
            ILogger<ServiceController> logger,
            IMediatorHandler mediator) : base(notifications, mediator)
        {
            _serviceService = serviceService;
            _notifications = notifications;
            _logger = logger;
        }

        [HttpGet("services")]
        public IActionResult GetServices()
        {
         
            return Response(_serviceService.GetServices());
        }

        [HttpPost("services/find")]
        public IActionResult FindLeads([FromBody] SearchViewModel model)
        {

            return Response(_serviceService.FindServices(model));
        }

    }
}