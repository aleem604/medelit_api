using Medelit.Common;

namespace Medelit.Domain.Commands
{
    public class AssignPermissionToRoleCommand : AuthBaseCommand
    {
        public long RoleId { get; set; }
        public long[] Permissions { get; set; }
        public eRecordStatus Status { get; set; }
    }
}
