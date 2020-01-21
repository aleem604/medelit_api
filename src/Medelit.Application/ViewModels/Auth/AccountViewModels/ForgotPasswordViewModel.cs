using System.ComponentModel.DataAnnotations;

namespace Medelit.Application
{
    public class ForgotPasswordViewModel
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }
    }
}
