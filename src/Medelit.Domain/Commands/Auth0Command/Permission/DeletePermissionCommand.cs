namespace Medelit.Domain.Commands
{
    public class DeletePermissionCommand : AuthBaseCommand
    {
        public long Id { get; set; }
    }
}
