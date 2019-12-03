namespace Medelit.Domain.Commands
{
    public class GetPermissionsCommand : AuthBaseCommand
    {
        public string NameFilter { get; set; }
    }
}
