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
        public string Password { get; set; }
    }
}
