using Gugleus.Core.Domain.Dictionaries;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Gugleus.Core.Domain.Requests
{
    [Table("requests_queue", Schema = "he")]
    public class RequestQueue
    {
        [Key]
        [Column("id")]
        public long RequestId { get; set; }
        public Request Request { get; set; }

        [Column("id_status")]
        public string StatusCode { get; set; }
        public RequestStatus Status { get; set; }

        [Column("add_date")]
        public DateTime AddDate { get; set; }

        [Column("process_start_date")]
        public DateTime? ProcessStartDate { get; set; }

        [Column("process_end_date")]
        public DateTime? ProcessEndDate { get; set; }

        [Column("error_msg")]
        public string ErrorMsg { get; set; }
    }
}
