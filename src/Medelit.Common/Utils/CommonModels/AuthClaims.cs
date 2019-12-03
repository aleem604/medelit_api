using System;
using System.Collections.Generic;
using System.Text;

namespace Medelit.Common
{
  public class AuthClaims
    {
        public long Id { get; set; }
        public long UserId { get { return this.Id; } }
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
        public string VerificationToken { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}
