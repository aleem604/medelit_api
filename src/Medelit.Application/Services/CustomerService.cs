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

namespace Medelit.Application
{
    public class CustomerService : BaseService, ICustomerService
    {
        private readonly ICustomerRepository _customerRepository;
        private readonly ILanguageRepository _langRepository;
        private readonly IStaticDataRepository _staticRepository;
        private readonly IServiceRepository _serviceRepository;
        private readonly IProfessionalRepository _professoinalRepository;
        private readonly IMapper _mapper;
        private readonly IConfiguration _configuration;
        private readonly IHttpContextAccessor _httpContext;
        private readonly IMediatorHandler _bus;

        public CustomerService(IMapper mapper,
            ApplicationDbContext context,
                            IHttpContextAccessor httpContext,
                            IConfiguration configuration,
                            IMediatorHandler bus,
                            ICustomerRepository customerRepository,
                            ILanguageRepository langRepository,
                           IStaticDataRepository staticRepository, 
                           IServiceRepository serviceRepository,
                            IProfessionalRepository professionalRepository):base(context)
{
            _mapper = mapper;
            _httpContext = httpContext;
            _configuration = configuration;
            _bus = bus;
            _customerRepository = customerRepository;
            _langRepository = langRepository;
            _staticRepository = staticRepository;
            _serviceRepository = serviceRepository;
            _professoinalRepository = professionalRepository;
        }

        public dynamic GetCustomers()
        {
            return _customerRepository.GetAll().Select(x => new { x.Id, x.TitleId, x.SurName }).ToList();
        }

        public dynamic FindCustomers(SearchViewModel viewModel)
        {
            viewModel.Filter = viewModel.Filter ?? new SearchFilterViewModel();

            var query = (from customer in _customerRepository.GetAll()
                         where customer.Status != eRecordStatus.Deleted
                         select customer)
                        .Select((x) => new
                        {
                            x.Id,
                            x.SurName,
                            x.Name,
                            //Age = x.DateOfBirth.HasValue ?  $"{Utils.GetAge(x.DateOfBirth).Item1} years and {Utils.GetAge(x.DateOfBirth).Item2} months" : string.Empty,
                            Age = x.DateOfBirth.HasValue ?  $"{Utils.GetAge(x.DateOfBirth).Item1} years" : string.Empty,
                            Email = x.Email,
                            Address = x.HomeStreetName,
                            x.MainPhone
                        });


            //if (!string.IsNullOrEmpty(viewModel.Filter.Search))
            //{
            //    viewModel.Filter.Search = viewModel.Filter.Search.Trim();
            //    query = query.Where(x =>
            //    (
            //        (!string.IsNullOrEmpty(x.Name) && x.Name.CLower().Contains(viewModel.Filter.Search.CLower()))
            //    || (x.SurName.Equals(viewModel.Filter.Search))
            //    || (!string.IsNullOrEmpty(x.Email) && x.Email.CLower().Contains(viewModel.Filter.Search.CLower()))
            //    || (x.Id.ToString().Contains(viewModel.Filter.Search))

            //    ));
            //}

            switch (viewModel.SortField)
            {
                //case "name":
                //    if (viewModel.SortOrder.Equals("asc"))
                //        query = query.OrderBy(x => x.Name);
                //    else
                //        query = query.OrderByDescending(x => x.Name);
                //    break;

                //case "surname":
                //    if (viewModel.SortOrder.Equals("asc"))
                //        query = query.OrderBy(x => x.SurName);
                //    else
                //        query = query.OrderByDescending(x => x.SurName);
                //    break;

                //case "email":
                //    if (viewModel.SortOrder.Equals("asc"))
                //        query = query.OrderBy(x => x.Email);
                //    else
                //        query = query.OrderByDescending(x => x.Email);
                //    break;

                //case "age":
                //    if (viewModel.SortOrder.Equals("asc"))
                //        query = query.OrderBy(x => x.Age);
                //    else
                //        query = query.OrderByDescending(x => x.Age);
                //    break;

                default:
                    if (viewModel.SortOrder.Equals("asc"))
                        query = query.OrderBy(x => x.Id);
                    else
                        query = query.OrderByDescending(x => x.Id);

                    break;
            }

            var totalCount = query.LongCount();

            return new
            {
                items = query.Skip(viewModel.PageNumber * viewModel.PageSize).Take(viewModel.PageSize).ToList(),
                totalCount
            };
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