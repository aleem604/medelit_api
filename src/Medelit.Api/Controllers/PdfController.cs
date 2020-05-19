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
        private readonly IHostingEnvironment _hostingEnvironment;
        private readonly INotificationHandler<DomainNotification> _notifications;
        private readonly ILogger<PdfController> _logger;
        private readonly IMediatorHandler _bus;

        public PdfController(
            IGeneratePdf generatePdf,
            IHostingEnvironment hostingEnvironment,
            INotificationHandler<DomainNotification> notifications,
            ILogger<PdfController> logger,
            IMediatorHandler bus) : base(notifications, bus)
        {
            _generatePdf = generatePdf;
            _hostingEnvironment = hostingEnvironment;
            _notifications = notifications;
            _bus = bus;
            _logger = logger;
        }
        [AllowAnonymous]
        [HttpGet("pdf/generate-pdf/{invoiceId}")]
        public IActionResult GeneratePdf(long invoiceId)
        {
            var path = Path.Combine(_hostingEnvironment.WebRootPath, "assets","exicons", "150x150-Name-&-Logo.png");
            
            var html = $@"<html><body><h2>Test</h2>
                        <img src='data:image/png;base64, {GetBase64(path)}' width='50' height='50' />
                        </body></html>";
            var pdf = _generatePdf.GetPDF(html);
            var pdfStreamResult = new MemoryStream();
            pdfStreamResult.Write(pdf, 0, pdf.Length);
            pdfStreamResult.Position = 0;



            var inputStream = new MemoryStream();

            pdfStreamResult.CopyTo(inputStream);

            var fileBytes = inputStream.ToArray();

            var outputStream = new MemoryStream(fileBytes);

            return File(outputStream, "application/pdf");
        }

        private string GetBase64(string path)
        {
            byte[] imageArray = System.IO.File.ReadAllBytes(path);
            string base64ImageRepresentation = Convert.ToBase64String(imageArray);
            return base64ImageRepresentation;
        }



    }
}