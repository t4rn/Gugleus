using Gugleus.Core.Dto;
using Gugleus.Core.Results;

namespace Gugleus.Core.Services
{
    public interface IValidationService
    {
        MessageListResult ValidateNewPost(PostDto newPost);
    }
}
