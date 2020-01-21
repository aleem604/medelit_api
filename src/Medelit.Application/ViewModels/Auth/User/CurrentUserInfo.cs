using System;
using System.Collections.Generic;
using System.Text;

namespace Medelit.Application
{
    public class CurrentUserInfo
    {
        public string Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public IList<string> Roles { get; set; }
    }
}
