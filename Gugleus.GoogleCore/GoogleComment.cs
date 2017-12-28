using System.Collections.Generic;

namespace Gugleus.GoogleCore
{
    public class GoogleComment
    {
        public string Content { get; set; }

        public List<GoogleActor> Plusoners { get; set; }

        public GoogleActor Author { get; set; }
    }
}
