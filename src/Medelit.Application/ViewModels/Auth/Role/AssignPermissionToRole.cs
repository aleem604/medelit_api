namespace Medelit.Application
{
    public class AssignPermissionToRoleViewModel : AuthBaseViewModel
    {
        public long RoleId { get; set; }
        public long[] Permissions { get; set; }

    }
}
