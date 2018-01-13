using System;

namespace Gugleus.Core.Dto.Output
{
    public class JobStatDto
    {
        public string Type { get; set; }
        public string Status { get; set; }
        public long Amount { get; set; }
        public TimeSpan AvgProcessTime { get; set; }
    }
}
