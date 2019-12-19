using System;
using Medelit.Common;
using Medelit.Domain.Core.Commands;
using Medelit.Domain.Models;

namespace Medelit.Domain.Commands
{
    public class SaveInvoiceEntityCommand : Command
    {
        public InvoiceEntity Entity { get; set; }

        public override bool IsValid()
        {
            return true;
        }
    }
}
