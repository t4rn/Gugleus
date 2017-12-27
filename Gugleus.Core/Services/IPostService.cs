using Gugleus.Core.Dto;
using Gugleus.Core.Results;
using System.Threading.Tasks;

namespace Gugleus.Core.Services
{
    public interface IPostService
    {
        Task<IdResult<long>> AddPost(PostDto postDto);

        Task<RequestStatusDto> GetPostStatus(long id);
    }
}
