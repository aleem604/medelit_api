using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Medelit.Application;
using Medelit.Domain.Core.Bus;
using Medelit.Domain.Core.Notifications;

namespace Medelit.Api.Controllers
{
    public class CustomerController : ApiController
    {
        private readonly ICustomerService _customerService;
        private readonly INotificationHandler<DomainNotification> _notifications;
        private readonly ILogger<CustomerController> _logger;

        public CustomerController(
            ICustomerService customerService,
            INotificationHandler<DomainNotification> notifications,
            ILogger<CustomerController> logger,
            IMediatorHandler mediator) : base(notifications, mediator)
        {
            _customerService = customerService;
            _notifications = notifications;
            _logger = logger;
        }

        [HttpGet("customers")]
        public IActionResult GetCustomers()
        {
         
            return Response(_customerService.GetCustomers());
        }      

    }
}