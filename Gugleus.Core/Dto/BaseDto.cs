using System.ComponentModel.DataAnnotations;

namespace Gugleus.Core.Dto
{
    public abstract class BaseDto
    {
        [Required]
        public UserInfoDto User { get; set; }
    }
}
