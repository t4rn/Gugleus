using System.ComponentModel.DataAnnotations;

namespace Gugleus.Core.Dto.Input
{
    public class ImageDto
    {
        [Required]
        public string Content { get; set; }
        [Required]
        public string Format { get; set; }
    }
}
