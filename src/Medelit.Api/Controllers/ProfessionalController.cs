using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Medelit.Application;
using Medelit.Domain.Core.Bus;
using Medelit.Domain.Core.Notifications;
using Medelit.Common;
using System.Collections.Generic;
using Medelit.Common.Models;

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
            _professionalService.FindProfessionals(model);
            return Response();
        }

        [HttpPost("professionals")]
        [HttpPut("professionals")]
        public IActionResult SaveProvessional([FromBody] ProfessionalViewModel model)
        {
            _professionalService.SaveProvessional(model);
            return Response();
        }

        [HttpGet("professionals/{professionalId}")]
        public IActionResult GetProfessionalById(long professionalId)
        {
            _professionalService.GetProfessionalById(professionalId);
            return Response();
        }

        [HttpPut("professionals/update-status/{status}")]
        public IActionResult UpdateStatus([FromBody] IList<ProfessionalViewModel> fees, eRecordStatus status)
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

        [HttpGet("professionals/services-data-for-attach/{proId}")]
        public IActionResult GetServicesToAttachWithProfessional(long proId)
        {
            _professionalService.GetServicesToAttachWithProfessional(proId);
            return Response();
        }

        [HttpGet("professionals/services-for-connect-filter/{proId}")]
        public IActionResult GetServicesForConnectFilter(long proId)
        {
            _professionalService.GetServicesForConnectFilter(proId);
            return Response();
        }

        [HttpPost("professionals/attach-services-to-professional/{proId}")]
        public IActionResult AttachServicesToProfessional([FromBody]IEnumerable<long> serviceIds, long proId)
        {
            _professionalService.AttachServicesToProfessional(serviceIds, proId);
            return Response();
        }

        [HttpGet("professionals/connected-customers/{proId}")]
        public IActionResult GetConnectedCustomers(long proId)
        {
            return Response(_professionalService.GetConnectedCustomers(proId));
        }

        [HttpGet("professionals/connected-bookings/{proId}")]
        public IActionResult GetConnectedBookings(long proId)
        {
            return Response(_professionalService.GetConnectedBookings(proId));
        }

        [HttpGet("professionals/connected-invoices/{proId}")]
        public IActionResult GetConnectedInvoices(long proId)
        {
            return Response(_professionalService.GetConnectedInvoices(proId));
        }

        [HttpGet("professionals/connected-leads/{proId}")]
        public IActionResult GetConnectedLeads(long proId)
        {
            return Response(_professionalService.GetConnectedLeads(proId));
        }

        [HttpGet("professionals/professional-connected-services/{proId}")]
        public IActionResult GetProfessionalConnectedServices(long proId)
        {
            _professionalService.GetProfessionalConnectedServices(proId);
            return Response();
        }

        [HttpPost("professionals/detach-professional-connected-service/{proId}")]
        public IActionResult DetachProfessionalConnectedService([FromBody]IEnumerable<EditProfessionalServiceFeesModel> serviceIds, long proId)
        {
            _professionalService.DetachProfessionalConnectedService(serviceIds, proId);
            return Response();
        }


        // add fee to professional service
        [HttpGet("professionals/professional-service-detail/{professionalPtFeeRowId}/{professionalProFeeRowId}")]
        public IActionResult GetProfessionalServiceDetail(long professionalPtFeeRowId, long professionalProFeeRowId)
        {
            return Response(_professionalService.GetProfessionalServiceDetail(professionalPtFeeRowId, professionalProFeeRowId));
        }

        [HttpPost("professionals/professional-service-detail/{serviceId}/{proId}")]
        public IActionResult SaveProfessionalServiceDetail([FromBody] FullFeeViewModel model)
        {
            _professionalService.SaveProfessionalServiceDetail(model);
            return Response();
        }

        // add fee to professional service
        [HttpGet("professionals/fees-for-filter-to-attach-with-service-professional/{ptRelationRowId}/{proRelationRowId}")]
        public IActionResult GetFeesForFilterToConnectWithServiceProfessional(long ptRelationRowId, long proRelationRowId)
        {
            _professionalService.GetFeesForFilterToConnectWithServiceProfessional(ptRelationRowId, proRelationRowId);
            return Response();
        }


    }
}