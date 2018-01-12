using System;
using System.ComponentModel.DataAnnotations;

namespace Gugleus.Core.Dto.Input
{
    public class DateFilterDto
    {
        //[Required]
        public DateTime From { get; set; }
        //[Required]
        public DateTime To { get; set; }
    }
}
