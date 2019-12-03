using Medelit.Common;

namespace Medelit.Domain.Commands
{
    public class UpdatePermissionCommand : AuthBaseCommand
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public eRecordStatus Status { get; set; }
    }
}
