using System;
using TinCore.Application.Interfaces;
using TinCore.Application.ViewModels;
using TinCore.Domain.Core.Bus;
using TinCore.Domain.Core.Notifications;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace TinCore.Api.Controllers
{
    public class GeoCitiesController : ApiController
    {
        private readonly ILocationService _locationService;

        public GeoCitiesController(
            ILocationService locationService,
            INotificationHandler<DomainNotification> notifications,
            IMediatorHandler mediator) : base(notifications, mediator)
        {
            _locationService = locationService;
        }

        ///api/v1/geocities
        [HttpGet, Route("geocities")]
        [AllowAnonymous]
        public IActionResult Get(string version, long id)
        {
            return Response(_locationService.GetCities(version,id));
        }
        [HttpGet]
        [AllowAnonymous]
        [Route("geocities/{id}")]
        public IActionResult GetCities(string version, long id)
        {
            var customerViewModel = _locationService.GetCities(version, id);

            return Response(customerViewModel);
        }

        [HttpPost]
        [Route("geocities")]
        public IActionResult Post([FromBody]LocationViewModel locationView)
        {
            if (!ModelState.IsValid)
            {
                NotifyModelStateErrors();
                return Response(locationView);
            }
           
            return Response(locationView);
        }

        [HttpPut]
        public IActionResult Put([FromBody]LocationViewModel locationViewModel)
        {
            if (!ModelState.IsValid)
            {
                NotifyModelStateErrors();
                return Response(locationViewModel);
            }

            //_geoCitiesService.Update(locationViewModel);

            return Response(locationViewModel);
        }

        [HttpDelete]
        public IActionResult Delete(int id)
        {        
            return Response();
        }

        
    }
}
