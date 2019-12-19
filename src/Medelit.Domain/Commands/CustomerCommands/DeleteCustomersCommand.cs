using System;
using System.Collections.Generic;
using Medelit.Common;
using Medelit.Domain.Core.Commands;
using Medelit.Domain.Models;

namespace Medelit.Domain.Commands
{
    public class DeleteCustomersCommand : Command
    {
        public IEnumerable<long> CustomerIds { get; set; }
        public override bool IsValid()
        {
            return true;
        }
    }
}
