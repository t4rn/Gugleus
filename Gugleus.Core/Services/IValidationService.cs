using Gugleus.Core.Dto.Input;
using Gugleus.Core.Results;

namespace Gugleus.Core.Services
{
    public interface IValidationService
    {
        MessageListResult ValidateNewPost(PostDto newPost);
        MessageListResult ValidateGetPostDetails(RequestDetailsDto requestDetailsDto);
    }
}
