using System;
using Medelit.Common;
using Medelit.Domain.Core.Commands;
using Medelit.Domain.Models;

namespace Medelit.Domain.Commands
{
    public class SaveProfessionalCommand : Command
    {
        public Professional Model { get; set; }

        public override bool IsValid()
        {
            return true;
        }
    }
}
