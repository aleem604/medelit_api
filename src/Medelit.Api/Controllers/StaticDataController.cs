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