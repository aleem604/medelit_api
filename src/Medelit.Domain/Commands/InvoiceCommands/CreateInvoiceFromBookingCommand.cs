using Medelit.Domain.Core.Commands;
using System;
using System.Collections.Generic;
using System.Text;

namespace Medelit.Domain.Commands
{
   public class CreateInvoiceFromBookingCommand: Command
    {
        public long BookingId { get; set; }
        public override bool IsValid()
        {
            return true;
        }
    }
}
