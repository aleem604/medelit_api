namespace Medelit.Domain.Commands
{
    public class UpdateUserCommand : AuthBaseCommand
    {
        public string UserId { get; set; }
        public string PhoneNumber { get; set; }
        public bool? EmailVerified { get; set; }
        public string Email { get; set; }
        public dynamic AppMetadata { get; set; }
        public dynamic UserMetadata { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public bool? PhoneVerified { get; set; }
        public bool? VerifyEmail { get; set; }
        public bool? VerifyPassword { get; set; }
        public bool? VerifyPhoneNumber { get; set; }
        public bool? Blocked { get; set; }

    }
}