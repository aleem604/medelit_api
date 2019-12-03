using System;
using System.Collections.Generic;
using System.Text;

namespace Medelit.Application
{
   public class UserAssignRolesRequestViewModel : AuthBaseViewModel
    {
        public string UserId { get; set; }
        public string[] Roles { get; set; }
    }
}
