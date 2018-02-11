using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Gugleus.Core.Domain.Dictionaries
{
    public abstract class DictionaryItem
    {
        [Key]
        [Column("code")]
        public string Code { get; set; }
        [Column("description")]
        public string Description { get; set; }
        [Column("ghost")]
        public bool Ghost { get; set; }
        [Column("add_date")]
        public DateTime AddDate { get; set; }
    }
}
