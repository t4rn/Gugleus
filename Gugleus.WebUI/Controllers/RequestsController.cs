using AutoMapper;
using Gugleus.Core.Domain;
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

        [Route("[controller]/Dev/{page?}/{pageSize?}")]
        public async Task<IActionResult> Dev(int? page, int? pageSize)
        {
            // TODO: show all requests
            RequestListVM model = await PrepareRequestListVM(EnvType.Dev, page, pageSize);
            return View("RequestList", model);
        }

        [Route("[controller]/Rc/{page?}/{pageSize?}")]
        public async Task<IActionResult> Rc(int? page, int? pageSize)
        {
            RequestListVM model = await PrepareRequestListVM(EnvType.Rc, page, pageSize);
            return View("RequestList", model);
        }

        [Route("[controller]/Prod/{page?}/{pageSize?}")]
        public async Task<IActionResult> Prod(int? page, int? pageSize)
        {
            RequestListVM model = await PrepareRequestListVM(EnvType.Prod, page, pageSize);
            return View("RequestList", model);
        }

        private async Task<RequestListVM> PrepareRequestListVM(EnvType env, int? page, int? pageSize)
        {
            RequestListVM model = new RequestListVM();
            //var requests = (await _requestSrv.GetAllAsync(env)).OrderByDescending(x => x.Id);

            var requests = (await _requestSrv.GetAllQueryableAsync(env)).OrderByDescending(x => x.Id);

            var pageNumber = page ?? 1;
            var size = pageSize ?? 20;
            var onePageOfProducts = requests.ToPagedList(pageNumber, size)
                .ToMappedPagedList<Core.Domain.Requests.Request, RequestVM>();

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
            RequestVM requestVM = await PrepareRequestVM(id, env);

            return View(requestVM);
        }

        /// <summary>
        /// For Request details in Modal
        /// </summary>
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
