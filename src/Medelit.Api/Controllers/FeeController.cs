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
            return Response(_feeService.FindFees(model));
        }

        //api/v1/
        [HttpGet("fees/{feeId}")]
        public IActionResult GetFeeById(long feeId)
        {
            return Response(_feeService.GetFeeById(feeId));
        }


        //api/v1/
        [HttpDelete("fees/{feeId}")]
        public IActionResult DeleteOffer(long feeId)
        {
            _feeService.DeleteFees(new List<long> { feeId });
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
        public IActionResult DeleteAttractions([FromBody] IList<long> feeIds)
        {
            _feeService.DeleteFees(feeIds);
            return Response();
        }

        //api/v1/
        [HttpGet("fees/connected-services/{feeId}")]
        public IActionResult GetConnectedServices(long feeId)
        {
            return Response(_feeService.GetConnectedServices(feeId));
        }

        //api/v1/
        [HttpGet("fees/connected-professionals-customers/{feeId}")]
        public IActionResult GetConnectedProfessionalCustomers(long feeId)
        {
            return Response(_feeService.GetConnectedProfessionalsCustomers(feeId));
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

    }
}