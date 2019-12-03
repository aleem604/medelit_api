using Medelit.Common;
using Medelit.Domain.Core.Commands;

namespace Medelit.Domain.Commands
{
    public class UserCreateCommand : AuthBaseCommand
    {
        public long Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string PhoneNumber { get; set; }
        public string PrimaryAddress { get; set; }
        public string SecondaryAddress { get; set; }
        public eUserType UserType { get; set; }
        public string ImageUrl { get; set; }
    }
}