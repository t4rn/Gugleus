using Gugleus.Core.Utils;

namespace Gugleus.Core.Results
{
    public class MessageListResult : Result
    {
        public MessageList MessageList { get; set; }

        public MessageListResult()
        {
            MessageList = new MessageList();
        }
    }
}
