using System;

namespace Gugleus.Core.Domain.Requests
{
    public class RequestStat
    {
        public string Type { get; set; }
        public string Status { get; set; }
        public long Count { get; set; }
        public TimeSpan? Avg { get; set; }
    }
}
