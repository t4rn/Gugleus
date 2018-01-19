using Gugleus.Core.Domain;
using Gugleus.Core.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Gugleus.Api.Controllers
{
    public abstract class BaseController : Controller
    {
        private readonly ICacheService _cacheService;

        public BaseController(ICacheService cacheService)
        {
            _cacheService = cacheService;
        }
        /// <summary>
        /// Returns a ObjectResult with 500 code
        /// </summary>
        protected IActionResult InternalServerError(object obj)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, obj);
        }

        protected async Task<WsClient> GetWsClient()
        {
            var hash = Request.Headers["Hash"];

            List<WsClient> wsClients = await _cacheService.GetWsClientsAsync();

            return wsClients.FirstOrDefault(x => x.Hash == hash);
        }
    }
}
