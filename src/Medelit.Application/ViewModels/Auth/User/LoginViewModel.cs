using System.ComponentModel.DataAnnotations;

namespace Medelit.Application
{
    public class LoginViewModel : AuthBaseViewModel
    {
        [Required]
        [StringLength(50, MinimumLength = 4)]
        public string UserName { get; set; }
        [Required, EmailAddress]
        [StringLength(50, MinimumLength = 4)]
        public string Email { get; set; }
        [Required]
        [StringLength(50, MinimumLength = 4)]
        public string Password { get; set; }
    }
}
