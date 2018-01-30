using AutoMapper;
using Gugleus.Core.Domain;
using Gugleus.Core.Domain.Requests;
using Gugleus.Core.Dto.Input;
using Gugleus.Core.Dto.Output;

namespace Gugleus.Core.Mapping
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            //CreateMap<PostDto, Post>().ReverseMap();
            //CreateMap<UserInfoDto, UserInfo>().ReverseMap();
            //CreateMap<MessageListResult, ResultDto>();

            //CreateMap<RequestStat, SummaryDto>()
            //    .ForMember(dest => dest.AvgProcessTime, opt => opt.MapFrom(src => src.Avg));
        }
    }

    public class PostResolver : IValueResolver<PostDto, Post, int?>
    {
        public int? Resolve(PostDto source, Post destination, int? destMember, ResolutionContext context)
        {
            return 123;
        }
    }
}
