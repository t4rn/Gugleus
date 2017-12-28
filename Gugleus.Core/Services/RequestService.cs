using AutoMapper;
using Gugleus.Core.Domain;
using Gugleus.Core.Domain.Requests;
using Gugleus.Core.Dto.Input;
using Gugleus.Core.Dto.Output;
using Gugleus.Core.Repositories;
using System;
using System.Threading.Tasks;

namespace Gugleus.Core.Services
{
    public class RequestService : IRequestService
    {
        private readonly IRequestRepository _requestRepository;
        private readonly IMapper _mapper;
        private readonly IUtilsService _utilsService;

        public RequestService(IRequestRepository requestRepository, IMapper mapper, IUtilsService utilsService)
        {
            _requestRepository = requestRepository;
            _mapper = mapper;
            _utilsService = utilsService;
        }

        public async Task<IdResultDto<long>> AddRequest<T>(T requestDto)
            where T : AbstractRequestDto
        {
            IdResultDto<long> result = new IdResultDto<long>();

            try
            {
                // preparing request
                Request request = PrepareRequest(requestDto);

                // saving to db
                long id = await _requestRepository.AddRequestAsync(request);

                // preparing result
                if (id > 0)
                {
                    result.IsOk = true;
                    result.Id = id;
                    result.Message = "Request successfully added to queue.";
                }
                else
                {
                    result.Message = "Something went wrong while adding request to db...";
                }
            }
            catch (Exception ex)
            {
                result.Message = $"Exception occured: {ex.Message}";
            }

            return result;
        }

        public async Task<RequestResponseDto<T>> GetRequestResponse<T>(long id, DictionaryItem.RequestType requestType) where T : class
        {
            RequestResponseDto<T> result = new RequestResponseDto<T>();

            try
            {
                Request request = await _requestRepository.GetRequestWithQueueAsync(id, requestType.ToString());

                if (request != null)
                {
                    result = PrepareRequestResponse<T>(request);
                }
                else
                {
                    result.Error = $"Request with Id: {id} not found...";
                }
            }
            catch (Exception ex)
            {
                result.Error = $"Exception occured: {ex.Message}";
            }

            return result;
        }

        private Request PrepareRequest(AbstractRequestDto requestDto)
        {
            Request request = new Request();
            request.Type = new DictionaryItem(requestDto.RequestType);
            request.Input = _utilsService.SerializeToJson(requestDto);

            return request;
        }

        private RequestResponseDto<T> PrepareRequestResponse<T>(Request request)
        {
            RequestResponseDto<T> dto = new RequestResponseDto<T>();

            dto.Id = request.Id;
            dto.Status = request.Queue?.Status?.Code;
            dto.Error = request.Queue?.ErrorMsg;

            if (dto.Status == DictionaryItem.RequestStatus.DONE.ToString())
            {
                if (!string.IsNullOrWhiteSpace(request.Output))
                {
                    dto.Obj = _utilsService.DeserializeFromJson<T>(request.Output);
                }
                else
                {
                    string errMsg = "Empty json from db";
                    dto.Error = !string.IsNullOrWhiteSpace(dto.Error) ? $"{dto.Error} | {errMsg}" : errMsg;
                }
            }

            return dto;
        }
    }
}
