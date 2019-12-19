using System;
using System.Collections.Generic;
using Medelit.Common;
using Medelit.Domain.Core.Commands;
using Medelit.Domain.Models;

namespace Medelit.Domain.Commands
{
    public class UpdateCustomersStatusCommand : Command
    {
        public IEnumerable<Customer> Customers { get; set; }
        public eRecordStatus Status { get; set; }
        public override bool IsValid()
        {
            return true;
        }
    }
}
