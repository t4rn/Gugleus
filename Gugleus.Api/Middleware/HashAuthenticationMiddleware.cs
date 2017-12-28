using Gugleus.Core.Domain;
using Gugleus.Core.Results;
using Gugleus.Core.Services;
using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Gugleus.Api.Middleware
{
    public class HashAuthenticationMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ICacheService _cacheService;

        public HashAuthenticationMiddleware(RequestDelegate next, ICacheService cacheService)
        {
            _next = next;
            _cacheService = cacheService;
        }

        public async Task Invoke(HttpContext context)
        {
            string hashFromHeader = context.Request.Headers["Hash"];
            if (!string.IsNullOrWhiteSpace(hashFromHeader))
            {
                // validating hash
                Result validationResult = await ValidateHash(hashFromHeader);

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

        private async Task<Result> ValidateHash(string hashFromHeader)
        {
            Result result = new Result();

            List<WsClient> wsClients = await _cacheService.GetWsClientsAsync();

            if (wsClients.Exists(x => x.Hash == hashFromHeader))
            {
                result.IsOk = true;
            }

            return result;
        }
    }
}
