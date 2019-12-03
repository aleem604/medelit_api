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

namespace Medelit.Application
{
    public class CustomerService : ICustomerService
    {
        private readonly ICustomerRepository _customerRepository;
        private readonly IMapper _mapper;
        private readonly IConfiguration _configuration;
        private readonly IHttpContextAccessor _httpContext;
        private readonly IMediatorHandler _bus;

        public CustomerService(IMapper mapper,
                            IHttpContextAccessor httpContext,
                            IConfiguration configuration,
                            IMediatorHandler bus,
                            ICustomerRepository customerRepository)
        {
            _mapper = mapper;
            _httpContext = httpContext;
            _configuration = configuration;
            _bus = bus;
            _customerRepository = customerRepository;
        }
       
        public dynamic GetCustomers()
        {
            return _customerRepository.GetAll();
        }

        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }

    }
}