namespace Medelit.Domain.Commands
{
    public class GetPermissionsByRoleCommand : AuthBaseCommand
    {
        public long RoleId { get; set; }
    }
}
