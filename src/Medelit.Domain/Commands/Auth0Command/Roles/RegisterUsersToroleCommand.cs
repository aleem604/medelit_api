using System;
using System.Collections.Generic;
using System.Text;
using Medelit.Common;
using Medelit.Domain.Core.Commands;

namespace Medelit.Domain.Commands
{
    public class RegisterUsersToRoleCommand : AuthBaseCommand
    {
        public long RoleId { get; set; }
        public long[] UserIds { get; set; }
        public eRecordStatus Status { get; set; }
    }
}
