using Gugleus.Core.Dto.Input;
using Gugleus.Core.Extensions;
using Gugleus.Core.Results;

namespace Gugleus.Core.Services
{
    public class ValidationService : IValidationService
    {
        public MessageListResult ValidateGetPostDetails(RequestDetailsDto requestDetailsDto)
        {
            MessageListResult result = new MessageListResult();

            // validating
            if (requestDetailsDto == null)
            {
                result.MessageList.Add("Null input.");
            }
            else if (string.IsNullOrWhiteSpace(requestDetailsDto.Url))
            {
                result.MessageList.Add("Null url.");
            }
            else if (!requestDetailsDto.Url.IsValidUri())
            {
                result.MessageList.Add("Invalid url.");
            }

            // preparing result
            PrepareResult(result);

            return result;
        }

        public MessageListResult ValidateNewPost(PostDto newPost)
        {
            MessageListResult result = new MessageListResult();

            // validating
            if (newPost == null)
            {
                result.MessageList.Add("Null Post input.");
            }

            // preparing result
            PrepareResult(result);

            return result;
        }

        /// <summary>
        /// Filles result with IsOk/Message
        /// </summary>
        private void PrepareResult(MessageListResult result)
        {
            if (!result.MessageList.MsgsExist)
            {
                result.IsOk = true;
            }
            else
            {
                result.Message = $"Validation errors: {result.MessageList.GetMessages(";")}";
            }
        }
    }
}
