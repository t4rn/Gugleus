using Gugleus.Core.Domain.Dictionaries;
using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Gugleus.Core.Domain.Requests
{
    [Table("requests", Schema = "he")]
    public class Request
    {
        [Column("id")]
        public long Id { get; set; }

        [Column("id_ws_client")]
        public int WsClientId { get; set; }
        public WsClient WsClient { get; set; }

        [Column("id_request_type")]
        public string TypeCode { get; set; }
        public RequestType Type { get; set; }

        [Column("request_input")]
        public string Input { get; set; }

        [Column("request_output")]
        public string Output { get; set; }

        [Column("add_date")]
        public DateTime AddDate { get; set; }

        [Column("output_date")]
        public DateTime? OutputDate { get; set; }

        public RequestQueue Queue { get; set; }
    }
}
