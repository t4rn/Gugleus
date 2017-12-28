using Gugleus.Core.Results;
using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;

namespace Gugleus.Api.Middleware
{
    public class HashAuthenticationMiddleware
    {
        private readonly RequestDelegate _next;

        public HashAuthenticationMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context)
        {
            string hashFromHeader = context.Request.Headers["Hash"];
            if (!string.IsNullOrWhiteSpace(hashFromHeader))
            {
                // validating hash
                Result validationResult = ValidateHash(hashFromHeader);

                if (validationResult.IsOk)
                {
                    await _next.Invoke(context);
                }
                else
                {
                    // bad hash
                    context.Response.StatusCode = 401; //Unauthorized
                    return;
                }
            }
            else
            {
                // no authorization header
                context.Response.StatusCode = 401; //Unauthorized
                return;
            }
        }

        //TODO: get hashes from db and store in cache
        private Result ValidateHash(string hashFromHeader)
        {
            Result result = new Result();
            if (hashFromHeader == "abc")
            {
                result.IsOk = true;
            }

            return result;
        }
    }
}
