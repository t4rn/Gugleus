using System.Collections.Generic;

namespace Gugleus.Core.Domain
{
    public class Post
    {
        //public long Id { get; set; }
        public UserInfo User { get; set; }
        public string Content { get; set; }
        public string Place { get; set; }
        public List<string> Images { get; set; }
    }
}
