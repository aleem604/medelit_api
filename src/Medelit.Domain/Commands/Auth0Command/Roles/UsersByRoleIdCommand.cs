using System;
using System.Collections.Generic;
using System.Text;
using Medelit.Domain.Core.Commands;

namespace Medelit.Domain.Commands
{
    public class GetUsersByRoleIdCommand : AuthBaseCommand
    {
        public long Id { get; set; }
        public string RoleId { get; set; }
    }
}
