using Gugleus.Core.Dto;
using Gugleus.Core.Results;

namespace Gugleus.Core.Services
{
    public class ValidationService : IValidationService
    {
        public MessageListResult ValidateNewPost(PostDto newPost)
        {
            MessageListResult result = new MessageListResult();

            // validating
            if (newPost == null)
            {
                result.MessageList.Add("Null Post input.");
            }

            // preparing result
            if (!result.MessageList.MsgsExist)
            {
                result.IsOk = true;
            }
            else
            {
                result.Message = $"Validation errors: {result.MessageList.GetMessages(";")}";
            }

            return result;
        }
    }
}
