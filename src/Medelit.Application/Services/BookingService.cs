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

namespace Medelit.Application
{
    public class BookingService : BaseService, IBookingService
    {
        private readonly ICustomerRepository _customerRepository;
        private readonly IInvoiceEntityRepository _ieRepository;
        private readonly IBookingRepository _bookingRepository;
        private readonly ILanguageRepository _langRepository;
        private readonly IServiceRepository _serviceRepository;
        private readonly IProfessionalRepository _professionalRepository;
        private readonly IStaticDataRepository _dataRepository;
        private readonly IMapper _mapper;
        private readonly IConfiguration _configuration;
        private readonly IHttpContextAccessor _httpContext;
        private readonly IMediatorHandler _bus;

        public BookingService(IMapper mapper,
            ApplicationDbContext context,
                            IHttpContextAccessor httpContext,
                            IConfiguration configuration,
                            IMediatorHandler bus,
                            ICustomerRepository customerRepository,
                            IInvoiceEntityRepository ieRepository,
                            IBookingRepository bookingRepository,
                            ILanguageRepository langRepository,
                            IServiceRepository serviceRepository,
                            IProfessionalRepository professionalRepository,
                            IStaticDataRepository dataRepository

            ) : base(context)
        {
            _mapper = mapper;
            _httpContext = httpContext;
            _configuration = configuration;
            _bus = bus;
            _ieRepository = ieRepository;
            _customerRepository = customerRepository;
            _bookingRepository = bookingRepository;
            _langRepository = langRepository;
            _serviceRepository = serviceRepository;
            _professionalRepository = professionalRepository;
            _dataRepository = dataRepository;
        }

        public dynamic GetBookings()
        {
            return _bookingRepository.GetAll().Select(x => new { x.Id, x.Name }).ToList();
        }

        public dynamic FindBookings(SearchViewModel viewModel)
        {
            viewModel.Filter = viewModel.Filter ?? new SearchFilterViewModel();
            var invoicingEntities = _ieRepository.GetAll().ToList();
            var services = _serviceRepository.GetAll().ToList();
            var professionals = _professionalRepository.GetAll().ToList();
            var paymentMethods = _dataRepository.GetPaymentMethods().ToList();

            var query = (from b in _bookingRepository.GetAll().Where(x => x.Status != eRecordStatus.Deleted)
                         join c in _customerRepository.GetAll() on b.CustomerId equals c.Id

                         select new
                         {
                             b.Id,
                             Name  = $"{b.Name} {b.SrNo}",
                             Customer = $"{c.SurName} {c.Name}",
                             InvoicingEntity = b.InvoiceEntityId.HasValue ? invoicingEntities.FirstOrDefault(x => x.Id == b.InvoiceEntityId).Name : "",
                             Service = b.ServiceId > 0 ? services.FirstOrDefault(x => x.Id == b.ServiceId).Name : "",
                             Professional = b.ProfessionalId > 0 ? professionals.FirstOrDefault(x => x.Id == b.ProfessionalId).Name : "",
                             b.BookingDate,
                             VisitDate = b.VisitStartDate,
                             b.PaymentMethodId,
                             PaymentMethod = b.PaymentMethodId.HasValue ? paymentMethods.FirstOrDefault(x => x.Id == b.PaymentMethodId).Value : "",
                             b.PtFee,
                             b.BookingStatusId,
                             b.PaymentStatusId,
                             b.InsuranceCoverId,
                             b.InvoiceNumber
                         });


            if (!string.IsNullOrEmpty(viewModel.Filter.Search))
            {
                viewModel.Filter.Search = viewModel.Filter.Search.Trim();
                query = query.Where(x =>
                (
                    (!string.IsNullOrEmpty(x.Name) && x.Name.CLower().Contains(viewModel.Filter.Search.CLower()))
                || (!string.IsNullOrEmpty(x.Customer) && x.Customer.CLower().Contains(viewModel.Filter.Search.CLower()))
                || (!string.IsNullOrEmpty(x.InvoicingEntity) && x.InvoicingEntity.CLower().Contains(viewModel.Filter.Search.CLower()))
                || (!string.IsNullOrEmpty(x.Service) && x.Service.CLower().Contains(viewModel.Filter.Search.CLower()))
                || (!string.IsNullOrEmpty(x.Professional) && x.Professional.CLower().Contains(viewModel.Filter.Search.CLower()))
                || (!string.IsNullOrEmpty(x.PaymentMethod) && x.PaymentMethod.CLower().Contains(viewModel.Filter.Search.CLower()))
                || (!string.IsNullOrEmpty(x.PtFee.ToString()) && x.PtFee.ToString().CLower().Contains(viewModel.Filter.Search.CLower()))

                || (x.BookingDate.HasValue && x.BookingDate.Value.ToString("YYYY-MM-DD").CLower().Contains(viewModel.Filter.Search.CLower()))
                || (x.VisitDate.HasValue && x.VisitDate.Value.ToString("YYYY-MM-DD").CLower().Contains(viewModel.Filter.Search.CLower()))
                || (x.Id.ToString().Contains(viewModel.Filter.Search))

                ));
            }

            if (viewModel.Filter.BookingFilter == eBookingFilter.Pending)
            {
                query = query.Where(x => x.BookingStatusId.Value == (short?)eBookingStatus.PendingConfirmation);
            }
            else if (viewModel.Filter.BookingFilter == eBookingFilter.AwaitingPayment)
            {
                query = query.Where(x => x.BookingStatusId.Value == (short?)eBookingStatus.Confirmed && x.PaymentStatusId == (short?)ePaymentStatus.Pending);
            }
            else if (viewModel.Filter.BookingFilter == eBookingFilter.TodayVisits)
            {
                query = query.Where(x => x.VisitDate.HasValue && x.VisitDate.Value.ToString("YYYYMMDD").Equals(DateTime.UtcNow.ToString("YYYYMMDD"), StringComparison.InvariantCultureIgnoreCase));
            }
            else if (viewModel.Filter.BookingFilter == eBookingFilter.FutureVisits)
            {
                query = query.Where(x => x.VisitDate.HasValue && x.VisitDate.Value > DateTime.UtcNow);
            }
            else if (viewModel.Filter.BookingFilter == eBookingFilter.Delivered)
            {
                query = query.Where(x => x.VisitDate.HasValue && x.VisitDate.Value < DateTime.UtcNow);
            }
            else if (viewModel.Filter.BookingFilter == eBookingFilter.ToBeInvoicesPaid)
            {
                query = query.Where(x => x.PaymentStatusId == (short)ePaymentStatus.Paid && x.PaymentMethodId != (short)ePaymentMethods.Insurance && string.IsNullOrEmpty(x.InvoiceNumber));
            }
            else if (viewModel.Filter.BookingFilter == eBookingFilter.ToBeInvoicedNotPaid)
            {
                query = query.Where(x => x.PaymentStatusId != (short)ePaymentStatus.Paid && x.PaymentMethodId == (short)ePaymentMethods.Insurance && string.IsNullOrEmpty(x.InvoiceNumber));
            }

            switch (viewModel.SortField)
            {
                case "name":
                    if (viewModel.SortOrder.Equals("asc"))
                        query = query.OrderBy(x => x.Name);
                    else
                        query = query.OrderByDescending(x => x.Name);
                    break;

                case "customer":
                    if (viewModel.SortOrder.Equals("asc"))
                        query = query.OrderBy(x => x.Customer);
                    else
                        query = query.OrderByDescending(x => x.Customer);
                    break;

                case "invoicingentity":
                    if (viewModel.SortOrder.Equals("asc"))
                        query = query.OrderBy(x => x.InvoicingEntity);
                    else
                        query = query.OrderByDescending(x => x.InvoicingEntity);
                    break;


                case "service":
                    if (viewModel.SortOrder.Equals("asc"))
                        query = query.OrderBy(x => x.Service);
                    else
                        query = query.OrderByDescending(x => x.Service);
                    break;
                case "professional":
                    if (viewModel.SortOrder.Equals("asc"))
                        query = query.OrderBy(x => x.Professional);
                    else
                        query = query.OrderByDescending(x => x.Professional);
                    break;

                case "bookingdate":
                    if (viewModel.SortOrder.Equals("asc"))
                        query = query.OrderBy(x => x.BookingDate);
                    else
                        query = query.OrderByDescending(x => x.BookingDate);
                    break;

                case "visitdate":
                    if (viewModel.SortOrder.Equals("asc"))
                        query = query.OrderBy(x => x.VisitDate);
                    else
                        query = query.OrderByDescending(x => x.VisitDate);
                    break;
                case "paymentmethod":
                    if (viewModel.SortOrder.Equals("asc"))
                        query = query.OrderBy(x => x.PaymentMethod);
                    else
                        query = query.OrderByDescending(x => x.PaymentMethod);
                    break;

                case "ptfee":
                    if (viewModel.SortOrder.Equals("asc"))
                        query = query.OrderBy(x => x.PtFee);
                    else
                        query = query.OrderByDescending(x => x.PtFee);
                    break;
                default:
                    if (viewModel.SortOrder.Equals("asc"))
                        query = query.OrderBy(x => x.BookingDate);
                    else
                        query = query.OrderByDescending(x => x.BookingDate);

                    break;
            }

            var totalCount = query.LongCount();

            return new
            {
                items = query.Skip(viewModel.PageNumber * viewModel.PageSize).Take(viewModel.PageSize).ToList(),
                totalCount
            };
        }

        public BookingViewModel GetBookingById(long bookingId)
        {
            var viewModel = _mapper.Map<BookingViewModel>(_bookingRepository.GetById(bookingId));
            if (viewModel != null)
            {

                if (viewModel.ServiceId > 0)
                {
                    var vatId = _serviceRepository.GetAll().FirstOrDefault(x => x.Id == viewModel.ServiceId).VatId;
                    viewModel.TaxType = _dataRepository.GetVats().FirstOrDefault(x => x.Id == vatId)?.DecValue;
                }
                viewModel.InvoiceEntityName = _ieRepository.GetAll().FirstOrDefault(x => x.Id == viewModel.InvoiceEntityId)?.Name;
                var customer = _customerRepository.GetAll().FirstOrDefault(x => x.Id == viewModel.CustomerId);

                if (customer != null)
                    viewModel.CustomerName = $"{customer.SurName} {customer.Name}";
                viewModel.AssignedTo = GetAssignedUser(viewModel.AssignedToId);
                if (viewModel.InvoiceId.HasValue)
                    viewModel.InvoiceNumber = _bookingRepository.GetBookingInvoiceNumber(viewModel.InvoiceId.Value);
            }
            return viewModel;
        }

        public void SaveBooking(BookingViewModel viewModel)
        {
            var bookingModel = _mapper.Map<Booking>(viewModel);

            _bus.SendCommand(new SaveBookingCommand { Booking = bookingModel });
        }

        public void UpdateStatus(IEnumerable<BookingViewModel> bookings, eRecordStatus status)
        {
            _bus.SendCommand(new UpdateBookingsStatusCommand { Bookings = _mapper.Map<IEnumerable<Booking>>(bookings), Status = status });
        }

        public void DeleteBookings(IEnumerable<long> bookingIds)
        {
            _bus.SendCommand(new DeleteBookingsCommand { BookingIds = bookingIds });
        }

        public void ConvertToBooking(long bookingId)
        {
            _bus.SendCommand(new ConvertCustomerToBookingCommand { BookingId = bookingId });
        }

        public void CreateClones(long bookingId, short bookings)
        {
            _bus.SendCommand(new CreateCloneCommand { BookingId = bookingId, NumberOfClones = bookings });
        }

        public void CreateCycle(long bookingId, short bookings)
        {
            _bus.SendCommand(new CreateCycleCommand { BookingId = bookingId, NumberOfCycles = bookings });
        }

        public dynamic GetBookingCycleConnectedBookings(long bookingId)
        {
            return _bookingRepository.GetBookingCycleConnectedBookings(bookingId);
        }

        public dynamic BookingConnectedProfessional(long bookingId)
        {
            return _bookingRepository.BookingConnectedProfessional(bookingId);
        }
        public dynamic BookingConnectedInvoices(long bookingId)
        {
            return _bookingRepository.BookingConnectedInvoices(bookingId);
        }

        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }

    }
}