using Microsoft.AspNetCore.Identity;
using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Medelit.Infra.CrossCutting.Identity.Models
{
    // Add profile data for application users by adding properties to the ApplicationUser class
    public class MedelitUser : IdentityUser<string>
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }

        [NotMapped]
        public string FullName { get {
                return $"{FirstName} {LastName}";    
            } 
        }
    }
}
