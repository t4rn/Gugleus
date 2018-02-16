using System.ComponentModel.DataAnnotations;

namespace Gugleus.WebUI.Models.Accounts
{
    public class RegisterVM
    {
        [Required]
        [Display(Name = "User name")]
        public string UserName { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [StringLength(16, MinimumLength = 3, ErrorMessage = "Password must be between 3 and 16 chars long")]
        public string Password { get; set; }

        [Required]
        [Display(Name = "Repeat password")]
        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "Passwords do not match")]
        [StringLength(16, MinimumLength = 3, ErrorMessage = "Password must be between 3 and 16 chars long")]
        public string PasswordRepeat { get; set; }
    }
}
