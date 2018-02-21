using Gugleus.Core.Domain;
using X.PagedList;

namespace Gugleus.WebUI.Models.Logs
{
    public class FileLogListVM
    {
        public IPagedList<FileLogVM> Logs { get; set; }
        public EnvType? Env { get; internal set; }
        public string Description { get; internal set; }
    }
}
