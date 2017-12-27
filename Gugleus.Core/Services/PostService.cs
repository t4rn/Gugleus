using AutoMapper;
using Gugleus.Core.Domain;
using Gugleus.Core.Domain.Requests;
using Gugleus.Core.Dto;
using Gugleus.Core.Repositories;
using Gugleus.Core.Results;
using System;
using System.Threading.Tasks;

namespace Gugleus.Core.Services
{
    public class PostService : IPostService
    {
        private readonly IRequestRepository _requestRepository;
        private readonly IMapper _mapper;
        private readonly IUtilsService _utilsService;

        public PostService(IRequestRepository requestRepository, IMapper mapper, IUtilsService utilsService)
        {
            _requestRepository = requestRepository;
            _mapper = mapper;
            _utilsService = utilsService;
        }

        public async Task<IdResult<long>> AddPost(PostDto postDto)
        {
            IdResult<long> result = new IdResult<long>();

            try
            {
                // mapping
                Post post = _mapper.Map<Post>(postDto);

                if (post != null)
                {
                    // preparing request
                    Request request = PrepareRequest(post);

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
                else
                {
                    result.Message = "Error mapping Post.";
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

        private Request PrepareRequest(Post post)
        {
            Request request = new Request();
            request.Type = new DictionaryItem(DictionaryItem.RequestType.ADDPOST);
            request.Input = _utilsService.SerializeToJson(post);

            return request;
        }

        private RequestStatusDto PrepareRequestStatus(Request request)
        {
            RequestStatusDto dto = new RequestStatusDto();

            dto.Id = request.Id;
            dto.Status = request.Queue.Status.Code;
            if (dto.Status == DictionaryItem.RequestStatus.DONE.ToString())
            {
                dto.Url = GetUrlFromRequest(request.Output);
            }
            dto.Error = request.Queue.ErrorMsg;

            return dto;
        }

        // TODO: impement
        private string GetUrlFromRequest(string json)
        {
            return "http://www.asd.com";
        }
    }
}
