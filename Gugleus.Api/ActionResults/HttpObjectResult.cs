using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace Gugleus.Api.ActionResults
{
    public class HttpObjectResult : IActionResult
    {
        private object _obj;
        private readonly int _statusCode;

        public HttpObjectResult(object obj, int statusCode)
        {
            _obj = obj;
            _statusCode = statusCode;
        }

        public Task ExecuteResultAsync(ActionContext context)
        {
            
            context.HttpContext.Response.StatusCode = _statusCode;
            return Task.CompletedTask;
        }
    }
}
