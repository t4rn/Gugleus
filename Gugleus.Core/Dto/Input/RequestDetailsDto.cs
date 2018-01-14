using Gugleus.Core.Domain;
using Gugleus.Core.Extensions;
using Gugleus.Core.Results;
using System.ComponentModel.DataAnnotations;

namespace Gugleus.Core.Dto.Input
{
    public class RequestDetailsDto : AbstractRequestDto
    {
        [Url(ErrorMessage = "Invalid Url")]
        [RegularExpression(@"^(?i)(https:\/\/plus\.google\.com\/)\S+\/(posts)\/\S+", ErrorMessage = "Urls from plus.google.com allowed only.")]
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
            else if (!Url.ToLower().StartsWith("https://plus.google.com/"))
            {
                // done with Regex DataAnnotation
                result.MessageList.Add("Not google url.");
            }

            // preparing result
            PrepareResult(result);

            return result;
        }
    }
}
