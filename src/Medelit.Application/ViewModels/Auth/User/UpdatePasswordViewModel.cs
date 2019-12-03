using System.ComponentModel.DataAnnotations;

namespace Medelit.Application
{
    public class UpdatePasswordViewModel : AuthBaseViewModel
    {
        public string UserId { get; set; }

        [EmailAddress]
        public string Email { get; set; }

        [Required(ErrorMessage = "Old Password is required")]
        [StringLength(255, ErrorMessage = "Old Password must be between 6 and 255 characters", MinimumLength = 5)]
        [DataType(DataType.Password)]
        public string OldPassword { get; set; }

        [Required(ErrorMessage = "New Password is required")]
        [StringLength(255, ErrorMessage = "New Password must be between 6 and 255 characters", MinimumLength = 5)]
        [DataType(DataType.Password)]
        public string NewPassword { get; set; }

        [Required, DataType(DataType.Password), Compare("NewPassword")]
        public string ConfirmPassword { get; set; }

    }
}
