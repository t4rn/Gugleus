using Gugleus.Core.Domain.Requests;
using Gugleus.Core.Services;
using Gugleus.WebUI.Models;
using Microsoft.AspNetCore.Mvc;
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
        private readonly IRequestService _requestService;

        public RequestsController(IRequestService requestService)
        {
            _requestService = requestService;
        }

        public IActionResult Index()
        {
            return View();
        }

        public async Task<IActionResult> Dev()
        {
            RequestListVM model = new RequestListVM();
            model.Requests = await _requestService.GetRequestsAsync();

            ViewData["Message"] = "Your requests.";

            return View(model);
        }

        public IActionResult Rc()
        {
            ViewData["Message"] = "Your contact page.";

            return View();
        }

        public IActionResult Prod()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
