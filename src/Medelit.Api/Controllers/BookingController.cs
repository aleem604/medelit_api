using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Medelit.Application;
using Medelit.Domain.Core.Bus;
using Medelit.Domain.Core.Notifications;
using Medelit.Common;
using System.Collections.Generic;

namespace Medelit.Api.Controllers
{
    public class BookingController : ApiController
    {
        private readonly IBookingService _bookingService;
        private readonly INotificationHandler<DomainNotification> _notifications;
        private readonly ILogger<BookingController> _logger;

        public BookingController(
            IBookingService bookingService,
            INotificationHandler<DomainNotification> notifications,
            ILogger<BookingController> logger,
            IMediatorHandler mediator) : base(notifications, mediator)
        {
            _bookingService = bookingService;
            _notifications = notifications;
            _logger = logger;
        }

        [HttpPost("bookings/find")]
        public IActionResult FindBookings([FromBody] SearchViewModel model)
        {

            return Response(_bookingService.FindBookings(model));
        }

        [HttpGet("bookings")]
        public IActionResult GetBookings()
        {

            return Response(_bookingService.GetBookings());
        }

        [HttpGet("bookings/{bookingId}")]
        public IActionResult GetBookingById(long bookingId)
        {

            return Response(_bookingService.GetBookingById(bookingId));
        }

        [HttpPost("bookings")]
        [HttpPut("bookings")]
        public IActionResult SaveProvessional([FromBody] BookingViewModel model)
        {
            _bookingService.SaveBooking(model);
            return Response();
        }

        [HttpPut("bookings/update-status/{status}")]
        public IActionResult UpdateStatus([FromBody] IEnumerable<BookingViewModel> bookings, eRecordStatus status)
        {
            _bookingService.UpdateStatus(bookings, status);
            return Response();
        }

        //api/v1/
        [HttpDelete("bookings/{bookingId}")]
        public IActionResult DeleteBooking(long bookingId)
        {
            _bookingService.DeleteBookings(new List<long> {bookingId });
            return Response();
        }

        //api/v1/
        [HttpPut("bookings/delete")]
        public IActionResult DeleteBookings([FromBody] IEnumerable<long> bookingIds)
        {
            _bookingService.DeleteBookings(bookingIds);
            return Response();
        }

        [HttpGet("bookings/convert-booking/{bookingId}")]
        public IActionResult ConvertToBooking(long bookingId)
        {
            _bookingService.ConvertToBooking(bookingId);
            return Response();
        }


        [HttpGet("bookings/create-clones/{bookingId}/{bookings}")]
        public IActionResult CreateClones(long bookingId, short bookings)
        {
            _bookingService.CreateClones(bookingId, bookings);
            return Response();
        }

        [HttpGet("bookings/create-cycle/{bookingId}/{bookings}")]
        public IActionResult CreateCycle(long bookingId, short bookings)
        {
            _bookingService.CreateCycle(bookingId, bookings);
            return Response();
        }

        [HttpGet("bookings/booking-cycle-connected-bookings/{bookingId}")]
        public IActionResult GetBookingCycleConnectedBookings(long bookingId)
        {      
            return Response(_bookingService.GetBookingCycleConnectedBookings(bookingId));
        }

        [HttpGet("bookings/booking-connected-professionals/{bookingId}")]
        public IActionResult GetBookingConnectedProfessionals(long bookingId)
        {
            return Response(_bookingService.BookingConnectedProfessional(bookingId));
        }

        [HttpGet("bookings/booking-connected-invoices/{bookingId}")]
        public IActionResult GetBookingConnectedInvoices(long bookingId)
        {
            return Response(_bookingService.BookingConnectedInvoices(bookingId));
        }



    }
}