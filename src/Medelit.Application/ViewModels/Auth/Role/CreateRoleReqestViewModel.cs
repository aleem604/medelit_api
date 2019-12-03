using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Medelit.Application
{
   public class CreateRoleReqestViewModel : AuthBaseViewModel
    {
        [Required(ErrorMessage = "Role Name is Required")]
        public string Name { get; set; }
        [Required(ErrorMessage = "Role Description is Required")]
        public string Description { get; set; }
    }
}
