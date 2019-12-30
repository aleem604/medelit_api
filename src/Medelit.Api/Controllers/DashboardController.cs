using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Medelit.Application;
using Medelit.Domain.Core.Bus;
using Medelit.Domain.Core.Notifications;
using System;
using Medelit.Common;
using System.Linq;
using System.Collections.Generic;

namespace Medelit.Api.Controllers
{
    public class DashboardController : ApiController
    {
        private readonly IDashboardService _dashboardService;
        private readonly INotificationHandler<DomainNotification> _notifications;
        private readonly ILogger<DashboardController> _logger;

        public DashboardController(
            IDashboardService dashboardService,
            INotificationHandler<DomainNotification> notifications,
            ILogger<DashboardController> logger,
            IMediatorHandler mediator) : base(notifications, mediator)
        {
            _dashboardService = dashboardService;
            _notifications = notifications;
            _logger = logger;
        }


        [HttpGet("dashboard/stats")]
        public IActionResult GetDashboardStats()
        {
            return Response(_dashboardService.GetDashboardStats());
        }

        

    }
}