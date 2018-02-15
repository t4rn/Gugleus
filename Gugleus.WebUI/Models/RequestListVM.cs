using System.Collections.Generic;
using Gugleus.Core.Domain;

namespace Gugleus.WebUI.Models
{
    public class RequestListVM
    {
        public string Description { get; set; }
        public List<RequestVM> Requests { get; set; }
        public EnvType? Env { get; internal set; }
    }
}
