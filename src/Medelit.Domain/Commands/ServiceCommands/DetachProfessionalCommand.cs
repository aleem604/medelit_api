using Medelit.Domain.Core.Commands;
using System;
using System.Collections.Generic;
using System.Text;

namespace Medelit.Domain.Commands
{
    public class DetachProfessionalCommand : Command
    {
        public long ServiceId { get; set; }
        public long ProfessionalId { get; set; }

        public override bool IsValid()
        {
            return true;
        }
    }
}
