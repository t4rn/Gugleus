using AutoMapper;
using Gugleus.Core.Domain;
using Gugleus.Core.Dto.Input;
using Gugleus.Core.Dto.Output;
using Gugleus.Core.Results;

namespace Gugleus.Core.Mapping
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<PostDto, Post>().ReverseMap();
            CreateMap<UserInfoDto, UserInfo>().ReverseMap();
            CreateMap<MessageListResult, ResultDto>();
        }
    }
}
