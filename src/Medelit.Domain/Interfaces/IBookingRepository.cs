using System.Collections.Generic;
using Medelit.Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace Medelit.Domain.Interfaces
{
    public interface IBookingRepository : IRepository<Booking>
    {
        string GetBookingName(string name, string surName);
    }
}