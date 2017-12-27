using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Gugleus.Core.Dto
{
    public class PostDto : BaseDto
    {
        //public long Id { get; set; }
        [Required]
        public string Content { get; set; }
        public string Place { get; set; }
        public List<string> Images { get; set; }
    }
}
