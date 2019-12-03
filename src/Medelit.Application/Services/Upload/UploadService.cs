using System;
using System.Collections.Generic;
using AutoMapper;
using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Medelit.Domain.Core.Bus;
using Medelit.Domain.Interfaces;

namespace Medelit.Application
{
    public class UploadService : IUploadService
    {
        private readonly IConfiguration _configuration;
        private readonly IMapper _mapper;
        private readonly IMediatorHandler _bus;
        public UploadService(
            IMapper mapper,
            IConfiguration configuration,
            IMediatorHandler bus
            )
        {
            _configuration = configuration;
            _mapper = mapper;
            _bus = bus;
        }

        public IEnumerable<ImageUploadResult> UploadFiles(IEnumerable<IFormFile> files)
        {
            var result = new List<ImageUploadResult>();
            foreach (IFormFile file in files)
            {
                using (var fileStram = file.OpenReadStream())
                {
                    var uploadParams = new ImageUploadParams()
                    {
                        File = new FileDescription(file.FileName, fileStram),
                    };
                    var uploadResult = GetCloudinary.Upload(uploadParams);
                    result.Add(uploadResult);
                }
            }

            return result;
        }

        private Cloudinary GetCloudinary
        {
            get
            {
                var account = new Account
                {
                    Cloud = _configuration.GetValue<string>("cloudinary:cloudName"),
                    ApiKey = _configuration.GetValue<string>("cloudinary:apiKey"),
                    ApiSecret = _configuration.GetValue<string>("cloudinary:apiSecret"),
                };

                return new Cloudinary(account);
            }
        }

        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }
    }
}