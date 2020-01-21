using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Medelit.Common;

namespace Medelit.Application
{
    public interface IBookingService : IDisposable
    {
        dynamic GetBookings();
        dynamic FindBookings(SearchViewModel model);
        BookingViewModel GetBookingById(long leadId);
        void SaveBooking(BookingViewModel model);
        void UpdateStatus(IEnumerable<BookingViewModel> leads, eRecordStatus status);
        void DeleteBookings(IEnumerable<long> leadIds);
        void ConvertToBooking(long leadId);
        void CreateClones(long bookingId, short bookings);
        void CreateCycle(long bookingId, short bookings);
        dynamic GetBookingCycleConnectedBookings(long bookingId);
        dynamic BookingConnectedProfessional(long bookingId);
        dynamic BookingConnectedInvoices(long bookingId);
    }
}
