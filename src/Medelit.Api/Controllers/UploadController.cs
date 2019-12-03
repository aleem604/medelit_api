using Medelit.Domain.Core.Bus;
using Medelit.Domain.Core.Notifications;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Medelit.Application;
using Microsoft.AspNetCore.Http;
using System.Collections.Generic;

namespace Medelit.Api.Controllers
{
    public class UploadController : ApiController
    {
        private readonly IUploadService _uploadService;
        private readonly INotificationHandler<DomainNotification> _notifications;
        private readonly ILogger<UploadController> _logger;

        public UploadController(
            IUploadService uploadService,
            INotificationHandler<DomainNotification> notifications,
            ILogger<UploadController> logger,
            IMediatorHandler mediator) : base(notifications, mediator)
        {
            _uploadService = uploadService;
            _notifications = notifications;
            _logger = logger;
        }

        [HttpPost("resource/media")]       
        public IActionResult UploadMedias(IFormCollection files)
        {
            if (files == null || files.Files == null || !(files.Files.Count > 0))
            {
                //return new { url = "", code = -1 };
            }
            var uploadFiles = new List<IFormFile>();
            foreach (var file in files.Files)
            {
                uploadFiles.Add(file);
            }

            return Response(_uploadService.UploadFiles(uploadFiles));
        }      
    }
}
