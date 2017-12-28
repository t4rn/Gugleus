using System.Collections.Generic;

namespace Gugleus.GoogleCore
{
    public class GoogleInfo
    {
        /// <summary>
        /// Identifier from GOOGLE+
        /// </summary>
        public string Identifier { get; set; }

        /// <summary>
        /// URL From GOOGLE+
        /// </summary>
        public string URL { get; set; }

        public GoogleActor Author { get; set; }

        public List<GoogleActor> Plusoners { get; set; }

        public List<GoogleComment> Comments { get; set; }
    }
}
