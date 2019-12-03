namespace Medelit.Domain.Commands
{
    public class DeleteUserCommand : AuthBaseCommand
    {
        public long UserId { get; set; }

    }
}