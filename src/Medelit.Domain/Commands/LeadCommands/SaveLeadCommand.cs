using System;
using Medelit.Common;
using Medelit.Domain.Core.Commands;
using Medelit.Domain.Models;

namespace Medelit.Domain.Commands
{
    public class SaveLeadCommand : Command
    {
        public Lead Lead { get; set; }
        public long? FromCustomerId { get; set; }
        public override bool IsValid()
        {
            return true;
        }
    }
}
