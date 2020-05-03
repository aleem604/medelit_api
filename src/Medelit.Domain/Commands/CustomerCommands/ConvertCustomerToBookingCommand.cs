using System;
using System.Collections.Generic;
using Medelit.Common;
using Medelit.Domain.Core.Commands;
using Medelit.Domain.Models;

namespace Medelit.Domain.Commands
{
    public class ConvertCustomerToBookingCommand : Command
    {
        public Lead Lead { get; set; }
        public long CustomerId { get; set; }
        public long BookingId { get; set; }
        public override bool IsValid()
        {
            return true;
        }
    }
}
