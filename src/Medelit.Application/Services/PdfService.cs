using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Medelit.Common;
using Medelit.Domain.Commands;
using Medelit.Domain.Core.Bus;
using Medelit.Domain.Interfaces;
using System.Linq;
using System.Collections.Generic;
using Medelit.Domain.Models;
using Medelit.Infra.CrossCutting.Identity.Data;
using Amazon.S3;
using Amazon;
using System.IO;
using Wkhtmltopdf.NetCore;
using Amazon.S3.Model;
using Microsoft.AspNetCore.Hosting;
using System.Threading.Tasks;

namespace Medelit.Application
{
    public class PdfService : BaseService, IPdfService
    {
        private readonly IHostingEnvironment _hostingEnvironment;
        private IGeneratePdf _generatePdf;
        private IInvoiceRepository _invoiceRepository;
        private readonly AmazonS3Client _client;
        private readonly IMapper _mapper;
        private readonly IConfiguration _configuration;
        private readonly IHttpContextAccessor _httpContext;
        private readonly IMediatorHandler _bus;

        public PdfService(IMapper mapper,
            IInvoiceRepository invoiceRepository,
            IHostingEnvironment hostingEnvironment,
            IGeneratePdf generatePdf,
                            ApplicationDbContext context,
                            IHttpContextAccessor httpContext,
                            IHostingEnvironment env,
                            IConfiguration configuration,
                            IMediatorHandler bus
                           
            ) : base(context, httpContext, configuration, env)
        {
            _hostingEnvironment = hostingEnvironment;
            _invoiceRepository = invoiceRepository;
            _generatePdf = generatePdf;
            _mapper = mapper;
            _httpContext = httpContext;
            _configuration = configuration;
            _bus = bus;
            _client = new AmazonS3Client(AwsKey, AwsSecretKey, RegionEndpoint.USEast1);
        }

        public async Task<DocumentModel> GenerateAndSavePdf(long invoiceId)
        {
            (string, string) template = _invoiceRepository.GetInvoiceHtml(invoiceId);
   
            var pdf = _generatePdf.GetPDF(template.Item1);
            
            byte[] fileBytes = new Byte[pdf.Length];
            
            
            var fileName = DateTime.Now.ToString("MMddyyyyhhmmss") + $"{template.Item2.Replace(" ", "")}.pdf";

            PutObjectResponse response = null;

            using (var stream = new MemoryStream(pdf))
            {
                var request = new PutObjectRequest
                {
                    BucketName = BucketName,
                    Key = $"{BucketFolders.InvoiceFolder}/{fileName}",
                    InputStream = stream,
                    ContentType = "application/pdf",
                    CannedACL = S3CannedACL.PublicRead,
                };

                response = await _client.PutObjectAsync(request);
            };

            if (response.HttpStatusCode == System.Net.HttpStatusCode.OK)
            {
                return new DocumentModel
                {
                    Url = $"https://mwm-pdfbucket.s3.amazonaws.com/invoices/{fileName}",
                };
            }
            else
            {
                return new DocumentModel();
            }
        }

        public async Task<string> DownloadDoc(string folderName, string fileName)
        {

            string responseBody = "";
            try
            {
                var request = new GetObjectRequest
                {
                    BucketName = BucketName,
                    Key = $"{folderName}/{fileName}"
                };

                using (GetObjectResponse response = await _client.GetObjectAsync(request))
                using (Stream responseStream = response.ResponseStream)
                using (StreamReader reader = new StreamReader(responseStream))
                {
                    responseBody = reader.ReadToEnd();
                }
            }
            catch (AmazonS3Exception e)
            {
                return e.Message;
            }
            catch (Exception e)
            {
                return e.Message;
            }
            return responseBody;

        }


        public async Task<List<DocumentModel>> GetAllFiles(string folderName)
        {
            var filesList = await _client.ListObjectsAsync(BucketName, folderName);
            var files = filesList.S3Objects.Where(x => !x.Key.EndsWith($"{folderName}/"))
                        .Select(x => new DocumentModel
                        {
                            FolderName = folderName,
                            DocumentName = x.Key.Replace($"{folderName}/", ""),
                            DocumentExtension = Path.GetExtension(x.Key).Replace(".", ""),
                            UploadDate = x.LastModified,
                            Url = $"{GetUrlLeftPart}/download-doc/{folderName}/{Path.GetFileName(x.Key)}"

                        }).OrderByDescending(x => x.UploadDate).ToList();
            return files;
        }

        
        public dynamic RemoveObject(string fileName)
        {

            var request = new DeleteObjectRequest
            {
                BucketName = BucketName,
                Key = fileName
            };

            var response = _client.DeleteObjectAsync(request).Result;

            if (response.HttpStatusCode == System.Net.HttpStatusCode.OK)
            {
                return new DocumentModel
                {
                    Success = true,
                    FileName = fileName
                };
            }
            else
            {
                return new DocumentModel
                {
                    Success = false,
                    FileName = fileName
                };
            }
        }

        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }

    }
}