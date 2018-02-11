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
        private readonly IRequestRepository _requestRepository;
        private readonly IMapper _mapper;

        public RequestsController(IRequestRepository requestService, IMapper mapper)
        {
            _requestRepository = requestService;
            _mapper = mapper;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Dev()
        {
            RequestListVM model = new RequestListVM();
            var requests = _requestRepository.GetAll().OrderByDescending(x => x.Id);

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

        public IActionResult Details(long id)
        {
            RequestListVM model = new RequestListVM();
            var request = _requestRepository.GetRequestById(id);

            var requestVM = _mapper.Map<RequestVM>(request);

            return View(requestVM);
        }
    }
}
