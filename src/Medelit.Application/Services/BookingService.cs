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
using Microsoft.AspNetCore.Hosting;

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
                            IStaticDataRepository dataRepository,
                            IHostingEnvironment env

            ) : base(context, httpContext, configuration, env)
        {
            _mapper = mapper;
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

        public void FindBookings(SearchViewModel viewModel)
        {
            _bookingRepository.FindBookings(viewModel);
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