using AutoMapper;
using Gugleus.Core.Domain;
using Gugleus.WebUI.Models.Requests;
using Gugleus.WebUI.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

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

        public async Task<IActionResult> Dev()
        {
            // TODO: pagination
            RequestListVM model = await PrepareModel(EnvType.Dev);
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

        private async Task<RequestListVM> PrepareModel(EnvType env)
        {
            RequestListVM model = new RequestListVM();
            var requests = (await _requestSrv.GetAllAsync(env)).OrderByDescending(x => x.Id);

            model.Requests = _mapper.Map<List<RequestVM>>(requests);
            model.Env = env;
            model.Description = $"Requests from {model.Env}";

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
