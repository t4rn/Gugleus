using Gugleus.Core.Domain;
using Gugleus.Core.Results;
using Gugleus.Core.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Gugleus.Api.Middleware
{
    public class HashAuthenticationMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ICacheService _cacheService;
        private readonly ILogger<HashAuthenticationMiddleware> _log;

        public HashAuthenticationMiddleware(RequestDelegate next, ICacheService cacheService,
            ILogger<HashAuthenticationMiddleware> log)
        {
            _next = next;
            _cacheService = cacheService;
            _log = log;
        }

        public async Task Invoke(HttpContext context)
        {
            try
            {
                string hashFromHeader = context.Request.Headers["Hash"];
                if (!string.IsNullOrWhiteSpace(hashFromHeader))
                {
                    // validating hash
                    Result validationResult = await ValidateHash(hashFromHeader);

                    if (validationResult.IsOk)
                    {
                        await _next(context);
                    }
                    else
                    {
                        // bad hash
                        _log.LogDebug($"Auth failed for hash: {hashFromHeader}");
                        context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                        return;
                    }
                }
                else
                {
                    // no authorization header
                    context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                    return;
                }
            }
            catch (Exception ex)
            {
                _log.LogError($"[AuthMiddleware] Ex: {ex}");
                context.Response.StatusCode = StatusCodes.Status507InsufficientStorage;
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
