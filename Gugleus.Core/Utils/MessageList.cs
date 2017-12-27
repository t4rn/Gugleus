using System.Collections.Generic;

namespace Gugleus.Core.Utils
{
    public class MessageList
    {
        private List<string> _messages = new List<string>();

        public bool MsgsExist { get; private set; }

        /// <summary>
        /// Adds message to message list
        /// </summary>
        public void Add(string format, params object[] args)
        {
            MsgsExist = true;
            _messages.Add(string.Format(format, args));
        }

        /// <summary>
        /// Returns messages list
        /// </summary>
        public string GetMessages(string separator)
        {
            return string.Join(separator, _messages);
        }
    }
}
