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
using Medelit.Domain.Models;
using System.Linq;
using System.Collections.Generic;
using Medelit.Infra.CrossCutting.Identity.Data;
using Microsoft.AspNetCore.Hosting;

namespace Medelit.Application
{
    public class CustomerService : BaseService, ICustomerService
    {
        private readonly ICustomerRepository _customerRepository;
        private readonly IMapper _mapper;
        private readonly IMediatorHandler _bus;

        public CustomerService(IMapper mapper,
            ApplicationDbContext context,
                            IHttpContextAccessor httpContext,
                            IConfiguration configuration,
                            IMediatorHandler bus,
                            ICustomerRepository customerRepository,
                           IHostingEnvironment env) :
            base(context, httpContext, configuration, env)
        {
            _mapper = mapper;
            _bus = bus;
            _customerRepository = customerRepository;
        }

        public dynamic GetCustomers()
        {
            return _customerRepository.GetAll().Select(x => new { x.Id, x.TitleId, x.SurName }).ToList();
        }

        public void FindCustomers(SearchViewModel viewModel)
        {
            _customerRepository.FindCustomer(viewModel);
        }

        public CustomerViewModel GetCustomerById(long customerId)
        {
            var customer = _customerRepository.GetById(customerId);
            var vm = _mapper.Map<CustomerViewModel>(customer);
            vm.AssignedTo = GetAssignedUser(vm.AssignedToId);
            return vm;
        }

        public void SaveCustomer(CustomerViewModel viewModel)
        {
            var customerModel = _mapper.Map<Customer>(viewModel);
            _bus.SendCommand(new SaveCustomerCommand { Customer = customerModel });
        }

        public void UpdateStatus(IEnumerable<CustomerViewModel> customers, eRecordStatus status)
        {
            _bus.SendCommand(new UpdateCustomersStatusCommand { Customers = _mapper.Map<IEnumerable<Customer>>(customers), Status = status });
        }

        public void DeleteCustomers(IEnumerable<long> customerIds)
        {
            _bus.SendCommand(new DeleteCustomersCommand { CustomerIds = customerIds });
        }

        public void CreateBooking(CustomerViewModel viewModel)
        {
            _bus.SendCommand(new ConvertCustomerToBookingCommand { CustomerId = viewModel.Id });
        }

        public dynamic GetCustomerConnectedCustomers(long customerId)
        {
            return _customerRepository.GetCustomerConnectedCustomers(customerId);
        }
        public dynamic GetCustomerConnectedServices(long customerId)
        {
            return _customerRepository.GetCustomerConnectedServices(customerId);
        }
        public dynamic GetCustomerConnectedProfessionals(long customerId)
        {
            return _customerRepository.GetCustomerConnectedProfessionals(customerId);
        }
        public dynamic GetCustomerConnectedBookings(long customerId)
        {
            return _customerRepository.GetCustomerConnectedBookings(customerId);
        }
        public dynamic GetCustomerConnectedInvoices(long customerId)
        {
            return _customerRepository.GetCustomerConnectedInvoices(customerId);
        }
        public dynamic GetCustomerConnectedLeads(long customerId)
        {
            return _customerRepository.GetCustomerConnectedLeads(customerId);
        }

        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }

    }
}