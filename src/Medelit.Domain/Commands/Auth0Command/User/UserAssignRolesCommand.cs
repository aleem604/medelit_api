using System;
using System.Collections.Generic;
using System.Text;
using Medelit.Domain.Core.Commands;

namespace Medelit.Domain.Commands
{
    public class UserAssignRolesCommand : AuthBaseCommand
    {
        public string UserId { get; set; }
        public string[] Roles { get; set; }
    }
}
