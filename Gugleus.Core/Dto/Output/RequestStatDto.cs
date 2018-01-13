using System;
using System.Collections.Generic;

namespace Gugleus.Core.Dto.Output
{
    public class RequestStatDto<T>
    {
        public RequestStatDto()
        {
            Summary = new List<SummaryDto>();
        }

        public T Filter { get; set; }
        public List<JobStatDto> Jobs { get; set; }
        public List<SummaryDto> Summary { get; set; }
    }
}
