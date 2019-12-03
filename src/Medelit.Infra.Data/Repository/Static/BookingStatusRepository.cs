using System.Linq;
using Medelit.Domain.Interfaces;
using Medelit.Domain.Models;
using Medelit.Infra.Data.Context;
using Medelit.Infra.Data.Repository;
using Microsoft.EntityFrameworkCore;

namespace Equinox.Infra.Data.Repository
{
    public class BookingStatusRepository : Repository<BookingStatus>, IBookingStatusRepository
    {
        public BookingStatusRepository(MedelitContext context)
            : base(context)
        {

        }
    }
}
