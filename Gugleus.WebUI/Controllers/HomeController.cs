using Gugleus.WebUI.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Diagnostics;

namespace Gugleus.WebUI.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            // TODO: charts with server disk space
            return View();
        }

        [AllowAnonymous]
        public IActionResult Error()
        {
            // Get the details of the exception that occurred
            var exceptionFeature = HttpContext.Features.Get<IExceptionHandlerPathFeature>();

            // TODO: global exception handling with call stack and parameters
            _logger.LogError("GlobalEx for path: '{0}' >>>> {1}", exceptionFeature?.Path, exceptionFeature?.Error?.GetBaseException());

            return View(new ErrorViewModel
            {
                RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier,
                Path = exceptionFeature?.Path,
                Exception = exceptionFeature?.Error?.GetBaseException()?.ToString()
            });
        }
    }
}
