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

namespace Medelit.Application
{
    public class BookingService : IBookingService
    {
        private readonly ICustomerRepository _customerRepository;
        private readonly IInvoiceEntityRepository _ieRepository;
        private readonly IBookingRepository _bookingRepository;
        private readonly ITitleRepository _titleRepository;
        private readonly ILanguageRepository _langRepository;

        private readonly IMapper _mapper;
        private readonly IConfiguration _configuration;
        private readonly IHttpContextAccessor _httpContext;
        private readonly IMediatorHandler _bus;

        public BookingService(IMapper mapper,
                            IHttpContextAccessor httpContext,
                            IConfiguration configuration,
                            IMediatorHandler bus,
                            ICustomerRepository customerRepository,
                            IInvoiceEntityRepository ieRepository,
                            IBookingRepository bookingRepository,
                            ILanguageRepository langRepository,
                            ITitleRepository titleRepository
            
            )
        {
            _mapper = mapper;
            _httpContext = httpContext;
            _configuration = configuration;
            _bus = bus;
            _ieRepository = ieRepository;
            _customerRepository = customerRepository;
            _bookingRepository = bookingRepository;
            _titleRepository = titleRepository;
            _langRepository = langRepository;
        }
       
        public dynamic GetBookings()
        {
            return _bookingRepository.GetAll().Select(x=> new {x.Id, x.Name }).ToList();
        }

        public dynamic FindBookings(SearchViewModel viewModel)
        {
            viewModel.Filter = viewModel.Filter ?? new SearchFilterViewModel();
            var langs = _langRepository.GetAll().ToList();
       

            var query = (from b in _bookingRepository.GetAll().Where(x => x.Status != eRecordStatus.Deleted)
                         join c in _customerRepository.GetAll() on b.CustomerId equals c.Id
                         
                         select new {
                             b.Id,
                             Language = langs.FirstOrDefault(l => l.Id == b.VisitLanguageId).Name,
                             b.Name,
                             CustomerName = c.Name,
                             b.Email,
                             b.PhoneNumber,
                             b.CreateDate,
                             b.UpdateDate,
                             b.AssignedToId,
                             b.Status
                         });


            if (!string.IsNullOrEmpty(viewModel.Filter.Search))
            {
                viewModel.Filter.Search = viewModel.Filter.Search.Trim();
                query = query.Where(x =>
                (
                    (!string.IsNullOrEmpty(x.Name) && x.Name.CLower().Contains(viewModel.Filter.Search.CLower()))
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

                case "customername":
                    if (viewModel.SortOrder.Equals("asc"))
                        query = query.OrderBy(x => x.CustomerName);
                    else
                        query = query.OrderByDescending(x => x.CustomerName);
                    break;

                case "assignedto":
                    if (viewModel.SortOrder.Equals("asc"))
                        query = query.OrderBy(x => x.AssignedToId);
                    else
                        query = query.OrderByDescending(x => x.AssignedToId);
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

        public BookingViewModel GetBookingById(long bookingId)
        {
            var viewModel =  _mapper.Map<BookingViewModel>(_bookingRepository.GetWithInclude(bookingId));
            viewModel.InvoiceEntityName = _ieRepository.GetAll().FirstOrDefault(x => x.Id == viewModel.InvoiceEntityId)?.Name;
            viewModel.CustomerName = _customerRepository.GetAll().FirstOrDefault(x => x.Id == viewModel.CustomerId)?.Name;

            return viewModel;
        }

        public void SaveBooking(BookingViewModel viewModel)
        {
            var bookingModel = _mapper.Map<Booking>(viewModel);
            bookingModel.Services = _mapper.Map<ICollection<BookingServiceRelation>>(viewModel.Services);
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

        public void ConvertToBooking(long customerId)
        {
            _bus.SendCommand(new ConvertCustomerToBookingCommand { CustomerId = customerId });
        }

        public void CreateInvoice(long bookingId)
        {
            _bus.SendCommand(new CreateInvoiceCommand { BookingId = bookingId });
        }

        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }

    }
}