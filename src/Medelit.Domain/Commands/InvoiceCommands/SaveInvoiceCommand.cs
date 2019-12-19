using System;
using Medelit.Common;
using Medelit.Domain.Core.Commands;
using Medelit.Domain.Models;

namespace Medelit.Domain.Commands
{
    public class SaveInvoiceCommand : Command
    {
        public Invoice Invoice { get; set; }

        public override bool IsValid()
        {
            return true;
        }
    }
}
