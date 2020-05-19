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
using Microsoft.Extensions.Hosting;

namespace Medelit.Application
{
    public class PdfService : BaseService, IPdfService
    {
        private readonly AmazonS3Client _client;
        private readonly IMapper _mapper;
        private readonly IConfiguration _configuration;
        private readonly IHttpContextAccessor _httpContext;
        private readonly IMediatorHandler _bus;

        public PdfService(IMapper mapper,
                            ApplicationDbContext context,
                            IHttpContextAccessor httpContext,
                            IConfiguration configuration,
                            IMediatorHandler bus
                           
            ) : base(context, httpContext, configuration)
        {
            _mapper = mapper;
            _httpContext = httpContext;
            _configuration = configuration;
            _bus = bus;
           
        }

        public void GetBookings()
        {
            
        }

        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }

    }
}