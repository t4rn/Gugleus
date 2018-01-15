using System;
using System.Collections.Generic;
using System.Linq;

namespace Gugleus.Core.Dto.Output
{
    public class RequestTypeStatDto
    {
        public string Type { get; set; }
        public long RequestsTotal { get { return Summary.Sum(x => x.Count); } }
        public TimeSpan AvgProcessTime { get; set; }
        public List<SummaryDto> Summary { get; set; }

        public RequestTypeStatDto()
        {
            Summary = new List<SummaryDto>();
        }
    }
}
