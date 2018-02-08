using AutoMapper;
using Gugleus.Core.Domain.Requests;
using Gugleus.Core.Services;
using Gugleus.WebUI.Models;
using Gugleus.WebUI.Repositories;
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
        private readonly IRequestRepository _requestService;
        private readonly IMapper _mapper;

        public RequestsController(IRequestRepository requestService, IMapper mapper)
        {
            _requestService = requestService;
            _mapper = mapper;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Dev()
        {
            RequestListVM model = new RequestListVM();
            var requests = _requestService.GetAll();

            model.Requests = _mapper.Map<List<RequestVM>>(requests);
            ViewBag.Message = "Requests from Dev";

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
