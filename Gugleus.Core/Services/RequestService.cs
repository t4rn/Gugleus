using AutoMapper;
using Gugleus.Core.Domain;
using Gugleus.Core.Domain.Google;
using Gugleus.Core.Domain.Requests;
using Gugleus.Core.Dto;
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
                long id = await _requestRepository.AddRequest(request);

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

        public async Task<RequestStatusDto> GetPostStatus(long id)
        {
            RequestStatusDto result = new RequestStatusDto();

            try
            {
                Request request = await _requestRepository.GetRequestWithQueue(id);

                if (request != null)
                {
                    result = PrepareRequestStatus(request);
                }
                else
                {
                    result.Error = $"Post with Id: {id} not found...";
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

        private RequestStatusDto PrepareRequestStatus(Request request)
        {
            RequestStatusDto dto = new RequestStatusDto();

            dto.Id = request.Id;
            dto.Status = request.Queue?.Status?.Code;
            if (dto.Status == DictionaryItem.RequestStatus.DONE.ToString())
            {
                dto.Url = GetUrlFromRequest(request.Output);
            }
            dto.Error = request.Queue?.ErrorMsg;

            return dto;
        }

        private string GetUrlFromRequest(string json)
        {
            string url = null;

            PostUrlInfo postInfo = _utilsService.DeserializeFromJson<PostUrlInfo>(json);
            if (postInfo.IsOk)
            {
                url = postInfo.RequestedUrl;
            }
            else
            {
                url = $"Not Ok returned from runner...";
            }

            return url;
        }
    }
}
