using System.ComponentModel.DataAnnotations;

namespace Gugleus.Core.Dto.Input
{
    public class UserInfoDto
    {
        [Required]
        public string Username { get; set; }
        [Required]
        public string Password { get; set; }
        //[Required]
        public string AdditionalEmail { get; set; }
    }
}
