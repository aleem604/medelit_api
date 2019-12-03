using System;
using System.Collections.Generic;
using System.Text;

namespace Medelit.Application
{
   public class RoleReqestViewModel : AuthBaseViewModel
    {
        public long RoleId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public long [] UserIds { get; set; }
    }
}
