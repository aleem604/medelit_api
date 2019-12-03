namespace Medelit.Domain.Commands
{
    public class UpdateUserPasswordCommand : AuthBaseCommand
    {
        public string UserId { get; set; }
        public string Email { get; set; }
        public string OldPassword { get; set; }
        public string NewPassword { get; set; }
        public string ConfirmPassword { get; set; }
    }
}