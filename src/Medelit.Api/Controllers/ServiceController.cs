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

        [HttpGet("services/{serviceId}")]
        public IActionResult GetServiceById(long serviceId)
        {

            return Response(_serviceService.GetServiceById(serviceId));
        }



        [HttpPost("services")]
        [HttpPut("services")]
        public IActionResult SaveService([FromBody] ServiceViewModel viewModel)
        {
            _serviceService.SaveService(viewModel);
            return Response();
        }

        //api/v1/
        [HttpDelete("services/{serviceId}")]
        public IActionResult DeleteService(long serviceId)
        {
            _serviceService.DeleteServices(new List<long> { serviceId });
            return Response();
        }

        [HttpPut("services/update-status/{status}")]
        public IActionResult UpdateStatus([FromBody] IEnumerable<ServiceViewModel> services, eRecordStatus status)
        {
            _serviceService.UpdateStatus(services, status);
            return Response();
        }
        //api/v1/
        [HttpPut("services/delete")]
        public IActionResult DeleteAttractions([FromBody] IEnumerable<long> serviceIds)
        {
            _serviceService.DeleteServices(serviceIds);
            return Response();
        }


    }
}