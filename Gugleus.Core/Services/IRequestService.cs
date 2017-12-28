using Gugleus.Core.Dto;
using System.Threading.Tasks;

namespace Gugleus.Core.Services
{
    public interface IRequestService
    {
        Task<IdResultDto<long>> AddRequest<T>(T requestDto) where T : AbstractRequestDto;
        Task<RequestStatusDto> GetPostStatus(long id);
    }
}
