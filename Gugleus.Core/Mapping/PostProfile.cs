using AutoMapper;
using Gugleus.Core.Dto;
using Gugleus.Core.Posts;

namespace Gugleus.Core.Mapping
{
    public class PostProfile : Profile
    {
        public PostProfile()
        {
            CreateMap<AddPostVM, Post>();
        }
    }
}
