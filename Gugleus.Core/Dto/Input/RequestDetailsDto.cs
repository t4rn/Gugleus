using System.ComponentModel.DataAnnotations;
using Gugleus.Core.Domain;
using Gugleus.Core.Extensions;
using Gugleus.Core.Results;

namespace Gugleus.Core.Dto
{
    public class RequestDetailsDto : AbstractRequestDto
    {
        [Url(ErrorMessage = "Invalid Url")]
        public string Url { get; set; }

        internal override DictionaryItem.RequestType RequestType => DictionaryItem.RequestType.GETINFO;

        public override MessageListResult Validate()
        {
            MessageListResult result = new MessageListResult();

            // validating

            if (string.IsNullOrWhiteSpace(Url))
            {
                result.MessageList.Add("Null url.");
            }
            else if (!Url.IsValidUri())
            {
                result.MessageList.Add("Invalid url.");
            }

            // preparing result
            PrepareResult(result);

            return result;
        }
    }
}
