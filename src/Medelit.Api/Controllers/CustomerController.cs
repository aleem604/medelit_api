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

        [HttpPost("customers/find")]
        public IActionResult FindCustomers([FromBody] SearchViewModel model)
        {
            _customerService.FindCustomers(model);
            return Response();
        }

        [HttpGet("customers")]
        public IActionResult GetCustomers()
        {

            return Response(_customerService.GetCustomers());
        }

        [HttpGet("customers/{customerId}")]
        public IActionResult GetCustomerById(long customerId)
        {

            return Response(_customerService.GetCustomerById(customerId));
        }

        [HttpPost("customers")]
        [HttpPut("customers")]
        public IActionResult SaveProvessional([FromBody] CustomerViewModel model)
        {
            _customerService.SaveCustomer(model);
            return Response();
        }

        [HttpPut("customers/update-status/{status}")]
        public IActionResult UpdateStatus([FromBody] IEnumerable<CustomerViewModel> customers, eRecordStatus status)
        {
            _customerService.UpdateStatus(customers, status);
            return Response();
        }
        //api/v1/
        [HttpPut("customers/delete")]
        public IActionResult DeleteAttractions([FromBody] IEnumerable<long> feeIds)
        {
            _customerService.DeleteCustomers(feeIds);
            return Response();
        }

        //api/v1/
        [HttpPost("customers/create-booking")]
        public IActionResult CreateBooking([FromBody] CustomerViewModel viewModel)
        {
            _customerService.CreateBooking(viewModel);
            return Response();
        }

        [HttpGet("customers/customer-connected-customers/{customerId}")]
        public IActionResult GetCustomerConnectedCustomers(long customerId)
        {
            return Response(_customerService.GetCustomerConnectedCustomers(customerId));
        }

        [HttpGet("customers/customer-requested-services/{customerId}")]
        public IActionResult GetCustomerConnectedServices(long customerId)
        {
            return Response(_customerService.GetCustomerConnectedServices(customerId));
        }

        [HttpGet("customers/customer-connected-professionals/{customerId}")]
        public IActionResult GetCustomerConnectedProfessionals(long customerId)
        {
            return Response(_customerService.GetCustomerConnectedProfessionals(customerId));
        }

        [HttpGet("customers/customer-connected-bookings/{customerId}")]
        public IActionResult GetCustomerConnectedBookings(long customerId)
        {
            return Response(_customerService.GetCustomerConnectedBookings(customerId));
        }

        [HttpGet("customers/customer-connected-invoices/{customerId}")]
        public IActionResult GetCustomerConnectedInvoices(long customerId)
        {
            return Response(_customerService.GetCustomerConnectedInvoices(customerId));
        }

        [HttpGet("customers/customer-connected-leads/{customerId}")]
        public IActionResult GetCustomerConnectedLeads(long customerId)
        {
            return Response(_customerService.GetCustomerConnectedLeads(customerId));
        }
    }
}