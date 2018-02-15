using System.Collections.Generic;
using Gugleus.Core.Domain;

namespace Gugleus.WebUI.Models.Logs
{
    public class FileLogListVM
    {
        public List<FileLogVM> Logs { get; set; }
        public EnvType? Env { get; internal set; }
        public string Description { get; internal set; }
    }
}
