using System;

namespace Gugleus.Core.Domain.Requests
{
    public class Request
    {
        public long Id { get; set; }
        public WsClient WsClient { get; set; }
        public DictionaryItem Type { get; set; }
        public string Input { get; set; }
        public string Output { get; set; }
        public DateTime AddDate { get; set; }
        public DateTime OutputDate { get; set; }
    }
}
