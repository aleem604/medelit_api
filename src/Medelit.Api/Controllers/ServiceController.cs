﻿using MediatR;
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
            _serviceService.FindServices(model);
            return Response();
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

        [HttpGet("services/tags")]
        public IActionResult GetServiceTags()
        {
            _serviceService.GetServiceTags();
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

        #region service connect professionals

        [HttpGet("services/service-connected-professionals/{serviceid}")]
        public IActionResult GetServiceConnectedProfessionals(long serviceId)
        {
            _serviceService.GetServiceConnectedProfessionals(serviceId);
            return Response();
        }

        [HttpGet("services/professionals-with-fees-to-connect-with-service/{serviceid}")]
        public IActionResult GetProfessionalsWithFeesToConnectWithService(long serviceId)
        {
            _serviceService.GetProfessionalsWithFeesToConnectWithService(serviceId);
            return Response();
        }
        [HttpPost("services/professionals-to-connect-with-service/{serviceid}")]
        public IActionResult SaveProfessionalsWithFeesToConnectWithService([FromBody]IEnumerable<long> porfessionalIds, long serviceId)
        {
            _serviceService.SaveProfessionalsWithFeesToConnectWithService(porfessionalIds, serviceId);
            return Response();
        }

        [HttpPost("services/detach-professionals-from-service/{serviceId}")]
        public IActionResult RemoveProfessionalsFromService([FromBody]IEnumerable<long> model, long serviceId)
        {
            _serviceService.RemoveProfessionalsFromService(model, serviceId);
            return Response();
        }

        [HttpGet("services/get-service-professional-fee-row-detail/{rowId}")]
        public IActionResult GetServiceProfessionalFeeRowDetail(long rowId)
        {
            _serviceService.GetServiceProfessionalFeeRowDetail(rowId);
            return Response();
        }

        [HttpGet("services/get-service-professional-fee-fees-for-filter/{rowId}")]
        public IActionResult GetServiceProfessionalFeesForFilter(long rowId)
        {
            _serviceService.GetServiceProfessionalFeesForFilter(rowId);
            return Response();
        }

        [HttpPost("services/save-service-professional-fees/{rowId}")]
        public IActionResult SaveProfessionalServices([FromBody]ProfessionalConnectedServicesModel model, long rowId)
        {
            _serviceService.SaveProfessionalServicesFees(model, rowId);
            return Response();
        }


        #endregion service connect professionals

        #region service connect pt fees
        [HttpGet("services/service-connected-pt-fees/{serviceid}")]
        public IActionResult GetServiceConnectedPtFees(long serviceId)
        {
            _serviceService.GetServiceConnectedFees(serviceId, eFeeType.PTFee);
            return Response();
        }

        [HttpGet("services/service-connected-pro-fees/{serviceid}")]
        public IActionResult GetServiceConnectedProFees(long serviceId)
        {
            _serviceService.GetServiceConnectedFees(serviceId, eFeeType.PROFee);
            return Response();
        }

        [HttpGet("services/service-connected-pt-fees-to-attach/{serviceid}")]
        public IActionResult GetServiceConnectedPtFeesToConnect(long serviceId)
        {
            _serviceService.GetServiceConnectedFeesToConnect(serviceId, eFeeType.PTFee);
            return Response();
        }

        [HttpGet("services/service-connected-pro-fees-to-attach/{serviceid}")]
        public IActionResult GetServiceConnectedProFeesToConnect(long serviceId)
        {
            _serviceService.GetServiceConnectedFeesToConnect(serviceId, eFeeType.PROFee);
            return Response();
        }



        [HttpPost("services/service-connected-pt-fees-attach/{serviceid}")]
        public IActionResult SavePtFeesForService([FromBody]IEnumerable<long> model, long serviceId)
        {
            _serviceService.SaveFeesForService(model, serviceId, eFeeType.PTFee);
            return Response();
        }

        [HttpPost("services/service-connected-pro-fees-attach/{serviceid}")]
        public IActionResult SaveProFeesForService([FromBody]IEnumerable<long> model, long serviceId)
        {
            _serviceService.SaveFeesForService(model, serviceId, eFeeType.PROFee);
            return Response();
        }

        [HttpPost("services/service-connected-pt-fees-detach/{serviceId}")]
        public IActionResult DetachFeeFromService([FromBody]IEnumerable<long> model, long serviceId)
        {
            _serviceService.DetachFeeFromService(model, serviceId, eFeeType.PTFee);
            return Response();
        }

        [HttpPost("services/service-connected-pro-fees-detach/{serviceId}")]
        public IActionResult DetachProFeeFromService([FromBody]IEnumerable<long> model, long serviceId)
        {
            _serviceService.DetachFeeFromService(model, serviceId, eFeeType.PROFee);
            return Response();
        }

        #endregion service connect pt fees


        [HttpPost("services/services-add-update-fees")]
        public IActionResult AddUpdateFeeForService([FromBody] AddUpdateFeeToServiceViewModel viewModel)
        {
            _serviceService.AddUpdateFeeForService(viewModel);
            return Response();
        }

        [HttpPost("services/services-data-for-attach")]
        public IActionResult GetServiceProfessionals([FromBody]ServicFilterViewModel viewModel)
        {
            return Response(_serviceService.GetServiceProfessionals(viewModel));
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