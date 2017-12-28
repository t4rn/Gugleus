using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Gugleus.Api.Controllers
{
    public abstract class BaseController : Controller
    {
        /// <summary>
        /// Returns a ObjectResult with 500 code
        /// </summary>
        protected IActionResult InternalServerError(object obj)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, obj);
        }
    }
}
