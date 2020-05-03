using Medelit.Domain.Core.Commands;
using System;
using System.Collections.Generic;
using System.Text;

namespace Medelit.Domain.Commands
{
   public class DeleteInvoiceBookingCommand: Command
    {
        public long InvoiceId { get; set; }
        public long BookingId { get; set; }
        public override bool IsValid()
        {
            return true;
        }
    }
}
