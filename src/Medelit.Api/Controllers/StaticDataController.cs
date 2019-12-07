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
    public class StaticDataController : ApiController
    {
        private readonly IStaticDataService _dataService;
        private readonly INotificationHandler<DomainNotification> _notifications;
        private readonly ILogger<StaticDataController> _logger;

        public StaticDataController(
            IStaticDataService dataService,
            INotificationHandler<DomainNotification> notifications,
            ILogger<StaticDataController> logger,
            IMediatorHandler mediator) : base(notifications, mediator)
        {
            _dataService = dataService;
            _notifications = notifications;
            _logger = logger;
        }

        [HttpGet("static/professionals")]
        public IActionResult GetProfessionalsForFitler()
        {
            return Response(_dataService.GetProfessionalsForFitler());
        }

        [HttpGet("static/ptfees")]
        public IActionResult GePTFeesForFilter()
        {
            return Response(_dataService.GePTFeesForFilter());
        }

        [HttpGet("static/profees")]
        public IActionResult GeROFeesForFilter()
        {
            return Response(_dataService.GetPROFeesForFilter());
        }

        [HttpGet("static/durations")]
        public IActionResult GeDurationsForFilter()
        {
            return Response(_dataService.GetDurations());
        }

        [HttpGet("static/vats")]
        public IActionResult GeVatsForFilter()
        {
            return Response(_dataService.GetVats());
        }

        [HttpGet("static/fields")]
        public IActionResult GetFieldsForFilter()
        {

            return Response(_dataService.GetFieldsForFilter());
        }

        [HttpGet("static/categories")]
        public IActionResult GetSubCategoriesForFilter()
        {

            return Response(_dataService.GetSubCategoriesForFilter());
        }

        [HttpGet("static/application-methods")]
        public IActionResult GetApplicationMethods()
        {

            return Response(_dataService.GetApplicationMethods());
        }
        [HttpGet("static/application-means")]
        public IActionResult GetApplicationMeans()
        {

            return Response(_dataService.GetApplicationMeans());
        }
        [HttpGet("static/document-list-sent")]
        public IActionResult GetDocumentListSent()
        {
            return Response(_dataService.GetDocumentListSents());
        }

        [HttpGet("static/contract-status")]
        public IActionResult GetContractStatusOptions()
        {
            return Response(_dataService.GetContractStatusOptions());
        }


        [HttpGet("static/collaboration-codes")]
        public IActionResult GetCollaborationCodes()
        {

            return Response(_dataService.GetCollaborationCodes());
        }

        [HttpGet("static/accounting-codes")]
        public IActionResult GetAccountingCodes()
        {
         
            return Response(_dataService.GetAccountingCodes());
        }

        [HttpGet("static/statuses")]
        public IActionResult GetStatuses()
        {
            return Response(Enum.GetValues(typeof(eRecordStatus)).Cast<eRecordStatus>().Select(e => new KeyValuePair<string, int>(e.ToString(), (int)e))
                .Select((x) => new {
                    id = x.Value,
                    name = x.Key
                }).OrderBy(x => x.id));
        }

        [HttpGet("static/titles")]
        public IActionResult GetTitles()
        {
            return Response(_dataService.GetTitles());
        }

        [HttpGet("static/languages")]
        public IActionResult GetLanguages()
        {
            return Response(_dataService.GetLanguages());
        }

        [HttpGet("static/cities")]
        public IActionResult GetCities()
        {
            return Response(_dataService.GetCities());
        }

        [HttpGet("static/countries")]
        public IActionResult GetCountries()
        {
            return Response(_dataService.GetCountries());
        }

        [HttpGet("static/relationships")]
        public IActionResult GetRelationships()
        {
            return Response(_dataService.GetRelationships());
        }


    }
}