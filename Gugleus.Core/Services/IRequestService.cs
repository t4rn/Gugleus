using Gugleus.Core.Domain;
using Gugleus.Core.Dto.Input;
using Gugleus.Core.Dto.Output;
using Gugleus.Core.Results;
using System;
using System.Threading.Tasks;
using Gugleus.Core.Domain.Requests;
using System.Collections.Generic;

namespace Gugleus.Core.Services
{
    public interface IRequestService
    {
        Task<IdResultDto<long>> AddRequestAsync<T>(T requestDto, WsClient wsClient) where T : AbstractRequestDto;
        Task<ObjResult<RequestResponseDto<T>>> GetRequestResponseAsync<T>(long id, DictionaryItem.RequestType requestType) where T : class;
        Task<RequestSummaryDto<DateFilterDto>> GetStatsByDate(DateTime from, DateTime to);
        Task<List<Request>> GetRequestsAsync();
    }
}
