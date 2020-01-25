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
        
        [HttpPost("services/services-add-update-fees")]
        public IActionResult AddUpdateFeeForService([FromBody] AddUpdateFeeToServiceViewModel viewModel)
        {
            _serviceService.AddUpdateFeeForService(viewModel);
            return Response();
        }



        [HttpPost("services/services-data-for-attach")]
        public IActionResult GetProfessionalServices([FromBody]ServicFilterViewModel viewModel)
        {
            return Response(_serviceService.GetProfessionalServices(viewModel));
        }

        [HttpPost("services/save-professional-services/{proId}")]
        public IActionResult SaveProfessionalServices([FromBody]IEnumerable<long> serviceIds, long proId)
        {
            _serviceService.SaveProfessionalServices(serviceIds, proId);
            return Response();
        }

        [HttpGet("services/detach-professional/{serviceId}/{proId}")]
        public IActionResult DetachProfessional(long serviceId, long proId)
        {
            _serviceService.DetachProfessional(serviceId, proId);
            return Response();
        }

        [HttpGet("services/professionals-relations/{proId}")]
        public IActionResult GetProfessionalRelations(long proId)
        {
            return Response(_serviceService.GetProfessionalRelations(proId));
        }

        [HttpGet("services/professionals-fees-detail/{serviceId}")]
        public IActionResult GetProfessionalFeesDetail(long serviceId)
        {
            return Response(_serviceService.GetProfessionalFeesDetail(serviceId));
        }

        [HttpGet("services/service-connected-professionals/{serviceId}")]
        public IActionResult GetServiceConnectedProfessionals(long serviceId)
        {
            return Response(_serviceService.GetServiceConnectedProfessionals(serviceId));
        }
        
        [HttpGet("services/connected-customers-invoicing-entities/{serviceId}")]
        public IActionResult GetConnectedCustomersInvoicingEntities(long serviceId)
        {
            return Response(_serviceService.GetConnectedCustomersInvoicingEntities(serviceId));
        }


        [HttpGet("services/service-connected-bookings/{serviceId}")]
        public IActionResult GetConnectedBookings(long serviceId)
        {
            return Response(_serviceService.GetConnectedBookings(serviceId));
        }

        [HttpGet("services/service-connected-customers-invoices/{serviceId}")]
        public IActionResult GetServiceConnectedCustomersAndInvoices(long serviceId)
        {
            return Response(_serviceService.GetConnectedCustomerInvoices(serviceId));
        }

        [HttpGet("services/service-connected-leads/{serviceId}")]
        public IActionResult GetServiceConnectedLeads(long serviceId)
        {
            return Response(_serviceService.GetConnectedLeads(serviceId));
        }



    }
}