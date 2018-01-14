using Gugleus.Core.Domain;
using Gugleus.Core.Extensions;
using Gugleus.Core.Results;
using System.ComponentModel.DataAnnotations;

namespace Gugleus.Core.Dto.Input
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
            else if (!Url.StartsWith("https://plus.google.com/"))
            {
                result.MessageList.Add("Urls from plus.google.com allowed only.");
            }

            // preparing result
            PrepareResult(result);

            return result;
        }
    }
}
