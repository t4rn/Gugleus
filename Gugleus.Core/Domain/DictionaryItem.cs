using System;
using System.ComponentModel.DataAnnotations;

namespace Gugleus.Core.Domain
{
    public class DictionaryItem
    {
        public DictionaryItem()
        {

        }

        public DictionaryItem(RequestType type) : this()
        {
            Code = type.ToString();
        }
        [Key]
        public string Code { get; set; }
        public string Description { get; set; }
        public bool Ghost { get; set; }
        public DateTime AddDate { get; set; }

        public enum RequestType
        {
            ADDPOST, GETINFO
        }

        public enum RequestStatus
        {
            WAIT, PROC, DONE, ERR
        }
    }
}
