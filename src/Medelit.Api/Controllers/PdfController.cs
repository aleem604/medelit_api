using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Medelit.Application;
using Medelit.Domain.Core.Bus;
using Medelit.Domain.Core.Notifications;
using Medelit.Common;
using System.Collections.Generic;
using System.Threading.Tasks;
using Wkhtmltopdf.NetCore;
using System.IO;
using Microsoft.AspNetCore.Hosting;
using System;

namespace Medelit.Api.Controllers
{
    public class PdfController : ApiController
    {
        private IGeneratePdf _generatePdf;
        private readonly IPdfService _pdfService;
        private readonly IHostingEnvironment _hostingEnvironment;
        private readonly INotificationHandler<DomainNotification> _notifications;
        private readonly ILogger<PdfController> _logger;
        private readonly IMediatorHandler _bus;

        public PdfController(
            IGeneratePdf generatePdf,
            IPdfService pdfService,
            IHostingEnvironment hostingEnvironment,
            INotificationHandler<DomainNotification> notifications,
            ILogger<PdfController> logger,
            IMediatorHandler bus) : base(notifications, bus)
        {
            _pdfService = pdfService;
            _generatePdf = generatePdf;
            _hostingEnvironment = hostingEnvironment;
            _notifications = notifications;
            _bus = bus;
            _logger = logger;
        }
        [AllowAnonymous]
        [HttpGet("pdf/generate-pdf/{invoiceId}")]
        public async Task<IActionResult> GeneratePdf(long invoiceId)
        {
            var file = await _pdfService.GenerateAndSavePdf(invoiceId);
            return Response(file);

        }
        [AllowAnonymous]
        [HttpGet("download-doc/{folderName}/{fileName}")]
        public async Task<IActionResult> DownloadDoc(string folderName, string fileName)
        {
            var file = await _pdfService.DownloadDoc(folderName, fileName);

            return Content(file, "application/octet-stream");
        }


    }
}