using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Medelit.Application;
using Medelit.Domain.Core.Bus;
using Medelit.Domain.Core.Notifications;
using Medelit.Common;
using System.Collections.Generic;

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

        [HttpPost("fees")]
        [HttpPut("fees")]
        public IActionResult SaveFee([FromBody]FeeViewModel viewModel)
        {
            _feeService.SaveFee(viewModel);
            return Response();
        }

        [HttpGet("fees")]
        public IActionResult GetFees()
        {
            return Response(_feeService.GetFees());
        }

        [HttpPost("fees/find")]
        public IActionResult FindFees([FromBody] SearchViewModel model)
        {
            _feeService.FindFees(model);
            return Response();
        }

        //api/v1/
        [HttpGet("fees/{feeId}/{feeType}")]
        public IActionResult GetFeeById(long feeId, eFeeType feeType)
        {
            _feeService.GetFeeById(feeId, feeType);
            return Response();
        }

        //api/v1/
        [HttpDelete("fees/{feeId}/{feeType}")]
        public IActionResult DeleteOffer(long feeId, eFeeType feeType)
        {
            _feeService.DeleteFees(new List<FeeViewModel> { new FeeViewModel { Id = feeId, FeeTypeId = feeType } });
            return Response();
        }

        [HttpPut("fees/update-status/{status}")]
        public IActionResult UpdateStatus([FromBody] IList<FeeViewModel> fees, eRecordStatus status)
        {
            _feeService.UpdateStatus(fees, status);
            return Response();
        }
        //api/v1/
        [HttpPut("fees/delete")]
        public IActionResult DeleteAttractions([FromBody] IList<FeeViewModel> feeIds)
        {
            _feeService.DeleteFees(feeIds);
            return Response();
        }

        //api/v1/
        [HttpGet("fees/services-for-filter/{feeId}/{feeType}")]
        public IActionResult GetServicesToConnect(long feeId, eFeeType feeType)
        {
            _feeService.GetServiceToConnect(feeId, feeType);
            return Response();
        }

        //api/v1/
        [HttpGet("fees/professionals-for-filter/{serviceId}/{feeId}/{feeType}")]
        public IActionResult GetProfessionalsToConnect(long serviceId, long feeId, eFeeType feeType)
        {
            _feeService.GetProfessionalToConnect(serviceId, feeId, feeType);
            return Response();
        }

        //api/v1/
        [HttpGet("fees/attach-new-service-professional-to-fee/{serviceId}/{professionalId}/{feeId}/{feeType}")]
        public IActionResult GetProfessionalsToConnect(long serviceId, long professionalId, long feeId, eFeeType feeType)
        {
            _feeService.AttachNewServiceProfessionalToFee(serviceId, professionalId, feeId, feeType);
            return Response();
        }

        //api/v1/
        [HttpGet("fees/fee-connected-services/{feeId}/{feeType}")]
        public IActionResult GetConnectedServices(long feeId, eFeeType feeType)
        {
            _feeService.GetConnectedServices(feeId, feeType);
            return Response();
        }

        //api/v1/
        [HttpGet("fees/fee-connected-professionals/{feeId}/{feeType}")]
        public IActionResult GetConnectedProfessionals(long feeId, eFeeType feeType)
        {
            _feeService.GetConnectedProfessionals(feeId, feeType);
            return Response();
        }

        //api/v1/
        [HttpGet("fees/connected-professionals-customers/{feeId}")]
        public IActionResult GetConnectedProfessionalCustomers(long feeId)
        {
            return Response(_feeService.GetConnectedProfessionalsCustomers(feeId));
        }

        //api/v1/
        [HttpPost("fees/delete-connected-professionals/{feeId}/{feeType}")]
        public IActionResult DeleteConnectedProfessionals([FromBody]IEnumerable<long> prosIds, long feeId, eFeeType feeType)
        {
            _feeService.DeleteConnectedProfessionals(prosIds, feeId, feeType);
            return Response();
        }

        //api/v1/
        [HttpGet("fees/services-to-connect-with-fee/{feeId}")]
        public IActionResult GetServicesToConnectWithFee(long feeId)
        {
            return Response(_feeService.GetServicesToConnectWithFee(feeId));
        }

        //api/v1/
        [HttpPost("fees/services-to-connect-with-fee/{feeId}")]
        public IActionResult SaveServicesToConnectWithFee([FromBody] IEnumerable<long> serviceIds, long feeId)
        {
            _feeService.SaveServicesToConnectWithFee(serviceIds, feeId);
            return Response();
        }

        //api/v1/
        [HttpGet("fees/professional-to-connect-with-fee/{feeId}/{feeType}")]
        public IActionResult GetProfessionalToConnectWithFee(long feeId, eFeeType feeType)
        {
            _feeService.GetProfessionalToConnectWithFee(feeId, feeType);
            return Response();
        }

        //api/v1/
        [HttpPost("fees/professional-to-connect-with-fee/{feeId}/{feeType}")]
        public IActionResult SaveProfessionalToConnectWithFee([FromBody] IEnumerable<long> serviceIds, long feeId, eFeeType feeType)
        {
            _feeService.SaveProfessionlToConnectWithFee(serviceIds, feeId, feeType);
            return Response();
        }




    }
}