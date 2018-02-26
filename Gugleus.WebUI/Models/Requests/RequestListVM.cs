using Gugleus.Core.Domain;
using X.PagedList;

namespace Gugleus.WebUI.Models.Requests
{
    public class RequestListVM
    {
        public string Description { get; set; }
        public IPagedList<RequestVM> Requests { get; set; }
        public EnvType? Env { get; internal set; }
        public int PageSize { get; set; }
        public bool ShowingAll { get; set; }
    }
}
