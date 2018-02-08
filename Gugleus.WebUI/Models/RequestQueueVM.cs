using System;

namespace Gugleus.WebUI.Models
{
    public class RequestQueueVM
    {
        public long Id { get; set; }
        public string Status { get; set; }
        public DateTime AddDate { get; set; }
        public DateTime ProcessStartDate { get; set; }
        public DateTime ProcessEndDate { get; set; }
        public string ErrorMsg { get; set; }
    }
}
