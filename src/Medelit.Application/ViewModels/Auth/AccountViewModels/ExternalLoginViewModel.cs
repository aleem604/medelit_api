using System.ComponentModel.DataAnnotations;

namespace Medelit.Application
{
    public class ExternalLoginViewModel
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }
    }
}
