using System.ComponentModel.DataAnnotations.Schema;

namespace Gugleus.Core.Domain.Dictionaries
{
    [Table("dic_request_type", Schema = "he")]
    public class RequestType : DictionaryItem
    {
        private RequestType()
        {

        }
        public RequestType(RequestTypeCode code) : this()
        {
            Code = code.ToString();
        }

        public enum RequestTypeCode
        {
            ADDPOST, GETINFO
        }
    }
}
