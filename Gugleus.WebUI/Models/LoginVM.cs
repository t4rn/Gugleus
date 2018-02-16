using System.ComponentModel.DataAnnotations;

namespace Gugleus.WebUI.Models
{
    public class LoginVM
    {
        [Required]
        [Display(Name ="User name")]
        public string UserName { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [StringLength(16, MinimumLength = 3, ErrorMessage = "Password must be between 3 and 16 chars long")]
        public string Password { get; set; }
    }
}
