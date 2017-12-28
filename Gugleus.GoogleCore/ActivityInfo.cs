using System.Collections.Generic;

namespace Gugleus.GoogleCore
{
    public class ActivityInfo
    {
        /// <summary>
        /// Information from GOOGLE
        /// This will be filled by screen-scrapper
        /// </summary>
        public GoogleInfo Google { get; set; }

        /// <summary>
        /// Reference from client system
        /// </summary>
        public string ReferenceIdentifier { get; set; }

        /// <summary>
        /// Text content. Supportf formating like *bold*, *strikesomething*
        /// </summary>
        public string Content { get; set; }

        /// <summary>
        /// Address to place. This will be put into google and first result will be set.
        /// </summary>
        public string Place { get; set; }

        /// <summary>
        /// List of URLs to images.
        /// </summary>
        public List<string> Images { get; set; }

        /// <summary>
        /// List of URL to some content.
        /// </summary>
        public List<string> URLs { get; set; }
    }
}
