using System;
using Medelit.Common;
using Medelit.Domain.Core.Commands;
using Medelit.Domain.Models;

namespace Medelit.Domain.Commands
{
    public class BookingFromCustomerCommand : Command
    {
        public long CustomerId { get; set; }
        public long LeadId { get; set; }

        public override bool IsValid()
        {
            return true;
        }
    }
}
