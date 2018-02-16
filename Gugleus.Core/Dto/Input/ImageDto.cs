using System.ComponentModel.DataAnnotations;

namespace Gugleus.Core.Dto.Input
{
    public class ImageDto
    {
        [Required]
        [StringLength(6993772, ErrorMessage = "Image exceeds maximum file size of 5 MB")]
        public string Content { get; set; }
        [Required]
        public string Format { get; set; }
    }
}
