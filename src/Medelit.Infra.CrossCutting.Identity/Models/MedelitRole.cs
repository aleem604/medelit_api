using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Medelit.Infra.CrossCutting.Identity.Models
{
    public class MedelitRole : IdentityRole<string>
    {
        public MedelitRole()
        {

        }
        public MedelitRole(string roleName) : base(roleName)
        {
        }

        public string Text { get; set; }
      
    }
}
