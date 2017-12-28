using Gugleus.Core.Domain;
using Gugleus.Core.Dto;
using Gugleus.Core.Dto.Output;
using System.Threading.Tasks;

namespace Gugleus.Core.Services
{
    public interface IRequestService
    {
        Task<IdResultDto<long>> AddRequest<T>(T requestDto) where T : AbstractRequestDto;
        Task<RequestResponseDto<T>> GetRequestResponse<T>(long id, DictionaryItem.RequestType requestType) where T : class;
    }
}
