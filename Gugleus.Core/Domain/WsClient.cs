using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Gugleus.Core.Domain
{
    [Table("ws_clients", Schema = "he")]
    public class WsClient
    {
        [Column("id")]
        public int Id { get; set; }
        [Column("client_name")]
        public string Name { get; set; }
        [Column("hash")]
        public string Hash { get; set; }
        [Column("ghost")]
        public bool Ghost { get; set; }
        [Column("add_date")]
        public DateTime AddDate { get; set; }
    }
}
