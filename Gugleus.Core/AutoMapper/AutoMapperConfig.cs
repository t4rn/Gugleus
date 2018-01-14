using AutoMapper;
using Gugleus.Core.Domain.Requests;
using Gugleus.Core.Dto.Output;
using System.Collections.Generic;
using System.Linq;

namespace Gugleus.Core.Mapping
{
    public class AutoMapperConfig
    {
        public static IMapper Initialize()
        {
            return new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<RequestStat, SummaryDto>()
                .ForMember(dest => dest.AvgProcessTime, opt => opt.MapFrom(src => src.Avg));

                //cfg.CreateMap<List<RequestStat>, List<RequestTypeStatDto>>()
                //.ForAllMembers(options => options.ResolveUsing<RequestResolver>());

            })
            .CreateMapper();
        }
    }

    //internal class RequestResolver : IValueResolver<List<RequestStat>, List<RequestTypeStatDto>, object>
    //{
    //    public object Resolve(List<RequestStat> source, List<RequestTypeStatDto> destination, object destMember, ResolutionContext context)
    //    {
    //        var groupedByType = source.GroupBy(rs => rs.Type);

    //        foreach (var gr in groupedByType)
    //        {
    //            RequestTypeStatDto jobStat = new RequestTypeStatDto { Type = gr.Key };

    //            foreach (RequestStat item in gr)
    //            {
    //                SummaryDto s = new SummaryDto() { AvgProcessTime = item.Avg, Count = item.Count, Status = item.Status };
    //                jobStat.Summary.Add(s);
    //            }

    //            destination.Add(jobStat);
    //        }

    //        return destination;
    //    }
    //}
}
