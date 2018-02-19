using AutoMapper;
using Gugleus.Core.Domain;
using Gugleus.WebUI.AutoMapper;
using Gugleus.WebUI.Models.Requests;
using Gugleus.WebUI.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using X.PagedList;

namespace Gugleus.WebUI.Controllers
{
    public class RequestsController : Controller
    {
        private readonly IMapper _mapper;
        private readonly IRequestSrv _requestSrv;
        private readonly ILogger<RequestsController> _logger;

        public RequestsController(IMapper mapper, IRequestSrv requestSrv, ILogger<RequestsController> logger)
        {
            _mapper = mapper;
            _requestSrv = requestSrv;
            _logger = logger;
        }

        [Route("[controller]/Dev/{page?}/{pageSize?}")]
        public async Task<IActionResult> Dev(int? page, int? pageSize = 20)
        {
            // TODO: pagination
            RequestListVM model = await PrepareModel(EnvType.Dev, page, pageSize);
            return View("RequestList", model);
        }

        public async Task<IActionResult> Rc()
        {
            RequestListVM model = await PrepareModel(EnvType.Rc);
            return View("RequestList", model);
        }

        public async Task<IActionResult> Prod()
        {
            RequestListVM model = await PrepareModel(EnvType.Prod);
            return View("RequestList", model);
        }

        private async Task<RequestListVM> PrepareModel(EnvType env, int? page = null, int? pageSize = null)
        {
            RequestListVM model = new RequestListVM();
            //var requests = (await _requestSrv.GetAllAsync(env)).OrderByDescending(x => x.Id);

            var requests = (await _requestSrv.GetAllQueryableAsync(env)).OrderByDescending(x => x.Id);

            var pageNumber = page ?? 1; // if no page was specified in the querystring, default to the first page (1)
            var size = pageSize ?? 100;
            var onePageOfProducts = requests.ToPagedList(pageNumber, size)
                .ToMappedPagedList<Core.Domain.Requests.Request, RequestVM>();

            ViewBag.OnePageOfProducts = onePageOfProducts;

            model.Requests = onePageOfProducts; // _mapper.Map<IPagedList<RequestVM>>(onePageOfProducts);
            model.Env = env;
            model.Description = $"Requests from {model.Env}";
            model.PageSize = size;

            return model;
        }



        [Route("[controller]/Details/{env}/{id}")]
        public async Task<IActionResult> Details(long id, EnvType? env)
        {
            _logger.LogDebug($"[{nameof(Details)}] Start for id = '{id}' and env = '{env}'");

            var request = await _requestSrv.GetRequestByIdAsync(env.Value, id);

            RequestVM requestVM = _mapper.Map<RequestVM>(request);
            requestVM.Env = env;

            return View(requestVM);
        }
    }
}
