using AutoMapper;
using Gugleus.Core.Domain;
using Gugleus.Core.Domain.Requests;
using Gugleus.WebUI.AutoMapper;
using Gugleus.WebUI.Models.Requests;
using Gugleus.WebUI.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
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

        [Route("[controller]/All/{env}")]
        public async Task<IActionResult> All(EnvType env)
        {
            RequestListVM model = await PrepareRequestListVM(env);
            return View("RequestList", model);
        }

        [Route("[controller]/List/{env}/{page?}/{pageSize?}")]
        public async Task<IActionResult> List(EnvType env, int? page, int? pageSize)
        {
            RequestListVM model = await PrepareRequestListVM(env, page, pageSize);
            return View("RequestList", model);
        }

        private async Task<RequestListVM> PrepareRequestListVM(EnvType env)
        {
            RequestListVM model = new RequestListVM();

            var requests = (await _requestSrv.GetAllAsync(env)).OrderByDescending(x => x.Id);

            model.Requests = requests.ToPagedList().ToMappedPagedList<Request, RequestVM>();
            model.Env = env;
            model.Description = $"All Requests from {model.Env}";
            model.PageSize = 1;
            model.ShowingAll = true;

            return model;
        }

        private async Task<RequestListVM> PrepareRequestListVM(EnvType env, int? page, int? pageSize)
        {
            RequestListVM model = new RequestListVM();

            var requests = (await _requestSrv.GetAllQueryableAsync(env)).OrderByDescending(x => x.Id);

            var pageNumber = page ?? 1;
            var size = pageSize ?? 20;
            var onePageOfProducts = requests.ToPagedList(pageNumber, size)
                .ToMappedPagedList<Request, RequestVM>();

            model.Requests = onePageOfProducts;

            model.Env = env;
            model.Description = $"Requests from {model.Env}";
            model.PageSize = size;

            return model;
        }



        [Route("[controller]/Details/{env}/{id}")]
        public async Task<IActionResult> Details(long id, EnvType? env)
        {
            _logger.LogDebug($"[{nameof(Details)}] Start for id = '{id}' and env = '{env}'");
            RequestVM requestVM = await PrepareRequestVM(id, env);

            return View(requestVM);
        }

        /// <summary>
        /// For Request details in Modal
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> DetailsPartial(long id, EnvType? env)
        {
            _logger.LogDebug($"[{nameof(DetailsPartial)}] Start for id = '{id}' and env = '{env}'");
            RequestVM requestVM = await PrepareRequestVM(id, env);

            return PartialView("_DetailsPartial", requestVM);
        }

        private async Task<RequestVM> PrepareRequestVM(long id, EnvType? env)
        {
            var request = await _requestSrv.GetRequestByIdAsync(env.Value, id);

            RequestVM requestVM = _mapper.Map<RequestVM>(request);
            requestVM.Env = env;
            return requestVM;
        }
    }
}
