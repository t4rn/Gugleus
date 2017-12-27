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
                        result.Message = "Something went wrong while adding request...";
                    }
                }
                else
                {
                    result.Message = "Error mapping Post.";
                }
            }
            catch (Exception ex)
            {
                result.Message = $"Exception occured: {ex}";
            }

            return result;
        }

        public async Task<ObjResult<PostDto>> GetPost(long id)
        {
            ObjResult<PostDto> result = new ObjResult<PostDto>();
            Request request = await _requestRepository.GetRequest(id);

            if (request != null)
            {
                var postDto = _mapper.Map<PostDto>(request);
                result.Object = postDto;
                result.IsOk = true;
            }
            else
            {
                result.Message = $"Your post of Id: {id} is still in waiting queue...";
            }

            return result;
        }

        private Request PrepareRequest(Post post)
        {
            Request request = new Request();
            request.Type = new DictionaryItem(DictionaryItem.RequestTypes.ADDPOST);
            request.Input = _utilsService.SerializeToJson(post);

            return request;
        }
    }
}
