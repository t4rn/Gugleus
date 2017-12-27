using AutoMapper;
using Gugleus.Core.Domain;
using Gugleus.Core.Dto;
using Gugleus.Core.Repositories;
using Gugleus.Core.Results;
using System;
using System.Threading.Tasks;

namespace Gugleus.Core.Services
{
    public class PostService : IPostService
    {
        private readonly IPostRepository _postRepository;
        private readonly IMapper _mapper;

        public PostService(IPostRepository postRepository, IMapper mapper)
        {
            _postRepository = postRepository;
            _mapper = mapper;
        }

        public async Task<IdResult<long>> AddPost(PostDto postDto)
        {
            IdResult<long> result = new IdResult<long>();

            try
            {
                // mapping
                Post post = _mapper.Map<Post>(postDto);

                // saving to db
                long id = await _postRepository.AddPost(post);

                // preparing result
                if (id > 0)
                {
                    result.IsOk = true;
                    result.Id = id;
                    result.Message = "Post successfully added.";
                }
                else
                {
                    result.Message = "Something went wrong while adding post...";
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
            Post post = await _postRepository.GetPost(id);

            if (post != null)
            {
                var postDto = _mapper.Map<PostDto>(post);
                result.Object = postDto;
                result.IsOk = true;
            }
            else
            {
                result.Message = $"Your post of Id: {id} is still in waiting queue...";
            }

            return result;
        }
    }
}
