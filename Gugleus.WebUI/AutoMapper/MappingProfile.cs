using AutoMapper;
using Gugleus.Core.Domain.Requests;
using Gugleus.WebUI.Models;

namespace Gugleus.WebUI.AutoMapper
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<RequestQueue, RequestQueueVM>()
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status.Code));

            CreateMap<Request, RequestVM>()
                .ForMember(dest => dest.WsClient, opt => opt.MapFrom(src => src.WsClient.Name))
                .ForMember(dest => dest.RequestType, opt => opt.MapFrom(src => src.Type.Code));
        }
    }
}
