using AutoMapper;
using Gugleus.Core.Domain;
using Gugleus.WebUI.Models;
using Gugleus.WebUI.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gugleus.WebUI.Controllers
{
    public class RequestsController : Controller
    {
        private readonly IMapper _mapper;
        private readonly IRequestSrv _requestSrv;

        public RequestsController(IMapper mapper, IRequestSrv requestSrv)
        {
            _mapper = mapper;
            _requestSrv = requestSrv;
        }

        public IActionResult Index()
        {
            return View();
        }

        public async Task<IActionResult> Dev()
        {
            RequestListVM model = new RequestListVM();
            var requests = (await _requestSrv.GetAllAsync(EnvType.Dev)).OrderByDescending(x => x.Id);

            model.Requests = _mapper.Map<List<RequestVM>>(requests);
            ViewBag.Message = "Requests from Dev";

            return View("RequestList", model);
        }

        public async Task<IActionResult> Rc()
        {
            RequestListVM model = new RequestListVM();
            var requests = (await _requestSrv.GetAllAsync(EnvType.Rc)).OrderByDescending(x => x.Id);

            model.Requests = _mapper.Map<List<RequestVM>>(requests);
            ViewBag.Message = "Requests from Rc";

            return View("RequestList", model);
        }

        public IActionResult Prod()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        public async Task<IActionResult> Details(long id)
        {
            RequestListVM model = new RequestListVM();
            var request = await _requestSrv.GetRequestByIdAsync(EnvType.Dev, id);

            var requestVM = _mapper.Map<RequestVM>(request);

            return View(requestVM);
        }
    }
}
