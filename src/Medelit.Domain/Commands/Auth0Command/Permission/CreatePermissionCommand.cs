using Medelit.Common;

namespace Medelit.Domain.Commands
{
    public class CreatePermissionCommand : AuthBaseCommand
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public eRecordStatus Status { get; set; }
    }
}
