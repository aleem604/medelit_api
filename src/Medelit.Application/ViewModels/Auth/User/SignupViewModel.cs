using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;
using Medelit.Common;

namespace Medelit.Application
{
    public class SignupViewModel : AuthBaseViewModel
    {
        [Required]
        public string FirstName { get; set; }
        [Required]
        public string LastName { get; set; }
        public string UserName { get; set; }
        [Required, EmailAddress]
        public string Email { get; set; }
        [Required]
        public string Password { get; set; }
        public string PhoneNumber { get; set; }
        public string PrimaryAddress { get; set; }
        public string SecondaryAddress { get; set; }
        [Required]
        public eUserType UserType { get; set; }
        public string ImageUrl { get; set; }
        public string ConfirmationCode { get; set; }
    }
}
