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

namespace Medelit.Application
{
    public class CustomerService : ICustomerService
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
                            IHttpContextAccessor httpContext,
                            IConfiguration configuration,
                            IMediatorHandler bus,
                            ICustomerRepository customerRepository,
                            ILanguageRepository langRepository,
                           IStaticDataRepository staticRepository, 
                           IServiceRepository serviceRepository,
                            IProfessionalRepository professionalRepository)
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
            var langs = _langRepository.GetAll().ToList();
            var statics = _staticRepository.GetAll().ToList();
            var services = _serviceRepository.GetAll().Select(x => new FilterModel { Id = x.Id, Value = x.Name }).ToList();
            var professionals = _professoinalRepository.GetAll().Select(x => new FilterModel { Id = x.Id, Value = x.Name }).ToList();

            var query = (from customer in _customerRepository.GetAllWithService()
                         where customer.Status != eRecordStatus.Deleted
                         select customer)
                        .Select((x) => new
                        {
                            Id = x.Id,
                            Title = statics.FirstOrDefault(s => s.Id == x.TitleId).Titles,
                            Language = langs.FirstOrDefault(s => s.Id == x.LanguageId).Name,
                            Services = PopulateServices(x.Services, services),
                            Professionals = PopulateProfessionals(x.Services, professionals),
                            SurName = x.SurName,
                            Name = x.Name,
                            Email = x.Email,
                            MainPhone = x.MainPhone,
                            UpdateDate = x.UpdateDate,
                            Status = x.Status
                        });


            if (!string.IsNullOrEmpty(viewModel.Filter.Search))
            {
                viewModel.Filter.Search = viewModel.Filter.Search.Trim();
                query = query.Where(x =>
                (
                    (!string.IsNullOrEmpty(x.Name) && x.Name.CLower().Contains(viewModel.Filter.Search.CLower()))
                || (x.SurName.Equals(viewModel.Filter.Search))
                //|| (!string.IsNullOrEmpty(x.currency) && x.currency.CLower().Contains(viewModel.Filter.Search.CLower()))
                || (!string.IsNullOrEmpty(x.Email) && x.Email.CLower().Contains(viewModel.Filter.Search.CLower()))
                || (x.Id.ToString().Contains(viewModel.Filter.Search))

                ));

            }

            if (viewModel.Filter.Status != eRecordStatus.All)
            {
                query = query.Where(x => x.Status == viewModel.Filter.Status);
            }

            switch (viewModel.SortField)
            {
                case "name":
                    if (viewModel.SortOrder.Equals("asc"))
                        query = query.OrderBy(x => x.Name);
                    else
                        query = query.OrderByDescending(x => x.Name);
                    break;

                case "surname":
                    if (viewModel.SortOrder.Equals("asc"))
                        query = query.OrderBy(x => x.SurName);
                    else
                        query = query.OrderByDescending(x => x.SurName);
                    break;

                case "email":
                    if (viewModel.SortOrder.Equals("asc"))
                        query = query.OrderBy(x => x.Email);
                    else
                        query = query.OrderByDescending(x => x.Email);
                    break;
                case "language":
                    if (viewModel.SortOrder.Equals("asc"))
                        query = query.OrderBy(x => x.Language);
                    else
                        query = query.OrderByDescending(x => x.Language);
                    break;

                case "status":
                    if (viewModel.SortOrder.Equals("asc"))
                        query = query.OrderBy(x => x.Status);
                    else
                        query = query.OrderByDescending(x => x.Status);
                    break;
                //case "createDate":
                //    if (viewModel.SortOrder.Equals("asc"))
                //        query = query.OrderBy(x => x.CreateDate);
                //    else
                //        query = query.OrderByDescending(x => x.CreateDate);
                //    break;
                //case "createdBy":
                //    if (viewModel.SortOrder.Equals("asc"))
                //        query = query.OrderBy(x => x.CreatedBy);
                //    else
                //        query = query.OrderByDescending(x => x.CreatedBy);
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

        private string PopulateServices(ICollection<CustomerServiceRelation> services, List<FilterModel> oservices)
        {
            var query = from s in services
                        join
                        os in oservices on s.ServiceId equals os.Id
                        select os.Value;

            return string.Join(",", query.ToArray());
        }

        private string PopulateProfessionals(ICollection<CustomerServiceRelation> services, List<FilterModel> professionals)
        {
            var query = from s in services
                        join
                        os in professionals on s.ProfessionalId equals os.Id
                        select os.Value;

            return string.Join(",", query.ToArray());
        }

        public CustomerViewModel GetCustomerById(long customerId)
        {
            var customer = _customerRepository.GetById(customerId);
            return _mapper.Map<CustomerViewModel>(customer);
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

        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }

    }
}