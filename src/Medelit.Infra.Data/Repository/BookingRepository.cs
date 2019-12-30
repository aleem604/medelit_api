using System.Collections.Generic;
using System.Linq;
using Medelit.Domain.Interfaces;
using Medelit.Domain.Models;
using Medelit.Infra.Data.Context;
using Medelit.Infra.Data.Repository;
using Microsoft.EntityFrameworkCore;

namespace Medelit.Infra.Data.Repository
{
    public class BookingRepository : Repository<Booking>, IBookingRepository
    {
        public BookingRepository(MedelitContext context)
            : base(context)
        {}


        public string GetBookingName(string name, string surName)
        {
            name = $"{name} {surName}";
            var bookingCount = Db.Booking.Where(x => x.Name.StartsWith(name)).Count();

            return $"{name} {++ bookingCount}";
        }


    }
}
