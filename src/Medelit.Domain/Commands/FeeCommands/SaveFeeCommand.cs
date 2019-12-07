using System;
using Medelit.Common;
using Medelit.Domain.Core.Commands;
using Medelit.Domain.Models;

namespace Medelit.Domain.Commands
{
    public class SaveFeeCommand : Command
    {
        public Fee Fee { get; set; }

        public override bool IsValid()
        {
            return true;
        }
    }
}
