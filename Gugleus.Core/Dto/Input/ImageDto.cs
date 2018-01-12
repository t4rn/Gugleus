using System.ComponentModel.DataAnnotations;

namespace Gugleus.Core.Dto.Input
{
    public class ImageDto
    {
        [Required]
        public string Image { get; set; }
        [Required]
        public string Format { get; set; }
    }
}
