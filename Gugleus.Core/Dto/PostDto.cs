﻿using System.Collections.Generic;

namespace Gugleus.Core.Dto
{
    public class PostDto : BaseDto
    {
        public long Id { get; set; }
        public string Content { get; set; }
        public string Place { get; set; }
        public List<string> Images { get; set; }
    }
}
