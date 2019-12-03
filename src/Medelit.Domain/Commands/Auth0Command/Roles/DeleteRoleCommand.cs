using System;
using System.Collections.Generic;
using System.Text;
using Medelit.Domain.Core.Commands;

namespace Medelit.Domain.Commands
{
    public class DeleteRoleCommand : AuthBaseCommand
    {
        public long Id { get; set; }
    }
}
