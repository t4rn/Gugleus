using System.ComponentModel.DataAnnotations;

namespace Gugleus.Core.Dto
{
    public class UserInfoDto
    {
        [Required]
        public string Username { get; set; }
        [Required]
        public string Password { get; set; }
    }
}
