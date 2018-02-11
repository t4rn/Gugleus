using System.ComponentModel.DataAnnotations.Schema;

namespace Gugleus.Core.Domain.Dictionaries
{
    [Table("dic_request_status", Schema = "he")]
    public class RequestStatus : DictionaryItem
    {

        public enum RequestStatusCode
        {
            WAIT, PROC, DONE, ERR
        }
    }
}
