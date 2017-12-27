using System;

namespace Gugleus.Core.Domain
{
    public class DictionaryItem
    {
        public DictionaryItem(RequestTypes type)
        {
            Code = type.ToString();
        }

        public string Code { get; set; }
        public string Description { get; set; }
        public bool Ghost { get; set; }
        public DateTime AddDate { get; set; }

        public enum RequestTypes
        {
            ADDPOST, GETINFO
        }
    }
}
