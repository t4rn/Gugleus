using System.Collections.Generic;

namespace Gugleus.Core.Dto.Output
{
    public class RequestStatDto<T>
    {
        public T Filter { get; set; }
        public List<JobStatDto> Jobs { get; set; }
    }

    public class JobStatDto
    {
        public string Type { get; set; }
        public string Status { get; set; }
        public long Amount { get; set; }
    }
}
