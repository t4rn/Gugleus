using System;
using System.Collections.Generic;

namespace Gugleus.Core.Dto.Output
{
    public class RequestSummaryDto<T>
    {
        public RequestSummaryDto()
        {
            Jobs = new List<RequestTypeStatDto>();
        }

        public T Filter { get; set; }
        public List<RequestTypeStatDto> Jobs { get; set; }
    }
}
