using System;
using System.Collections.Generic;
using System.Text;
using Medelit.Domain.Core.Commands;

namespace Medelit.Domain.Commands
{
    public class GetRolesCommand : AuthBaseCommand
    {
        public string NameFilter { get; set; }
    }
}
