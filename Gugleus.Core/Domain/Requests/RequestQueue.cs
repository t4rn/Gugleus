using System;

namespace Gugleus.Core.Domain.Requests
{
    public class RequestQueue
    {
        public long Id { get; set; }
        public DictionaryItem Status { get; set; }
        public DateTime AddDate { get; set; }
        public DateTime ProcessStartDate { get; set; }
        public DateTime ProcessEndDate { get; set; }
        public string ErrorMsg { get; set; }
    }
}
