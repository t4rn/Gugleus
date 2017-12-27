using AutoMapper;
using Gugleus.Core.Domain;
using Gugleus.Core.Dto;

namespace Gugleus.Core.Mapping
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<PostDto, Post>().ReverseMap();
            CreateMap<UserInfoDto, UserInfo>().ReverseMap();
        }
    }
}
