using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Gugleus.Api.Controllers
{
    public abstract class BaseController : Controller
    {
        protected IActionResult InternalServerError(object obj)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, obj);
        }
    }
}
