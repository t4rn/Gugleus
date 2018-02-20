using AutoMapper;
using Gugleus.Core.Domain.Requests;
using Gugleus.WebUI.Models.Logs;
using Gugleus.WebUI.Models.Requests;
using System.Collections.Generic;
using System.IO;
using X.PagedList;

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

            CreateMap<FileInfo, FileLogVM>()
                .ForMember(dest => dest.FileName, opt => opt.MapFrom(src => src.Name))
                .ForMember(dest => dest.FileSize, opt => opt.ResolveUsing<FileSizeResolver>())
                .ForMember(dest => dest.ModificationDate, opt => opt.MapFrom(src => src.LastWriteTime.ToShortDateString()));
        }
    }

    public class FileSizeResolver : IValueResolver<FileInfo, FileLogVM, string>
    {
        public string Resolve(FileInfo source, FileLogVM destination, string destMember, ResolutionContext context)
        {
            string result = null;

            if (source.Length > 0)
            {
                result = $"{source.Length / 1024} kB";
            }

            return result;
        }
    }

    public static class Extensions
    {
        public static IPagedList<TDestination> ToMappedPagedList<TSource, TDestination>(this IPagedList<TSource> list)
        {
            IEnumerable<TDestination> sourceList = Mapper.Map<IEnumerable<TSource>, IEnumerable<TDestination>>(list);
            IPagedList<TDestination> pagedResult = new StaticPagedList<TDestination>(sourceList, list.GetMetaData());
            return pagedResult;
        }
    }
}
