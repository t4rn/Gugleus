using System.Collections.Generic;
using System.Linq;

namespace Gugleus.Core.Dto.Output
{
    public class RequestTypeStatDto
    {
        public string Type { get; set; }
        public List<SummaryDto> Summary { get; set; }
        public long TotalRequests { get { return Summary.Sum(x => x.Count); } }

        public RequestTypeStatDto()
        {
            Summary = new List<SummaryDto>();
        }
    }
}
