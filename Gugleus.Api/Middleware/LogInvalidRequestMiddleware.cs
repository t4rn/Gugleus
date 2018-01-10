using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System;
using System.IO;
using System.Threading.Tasks;

namespace Gugleus.Api.Middleware
{
    public class LogInvalidRequestMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger _logger;

        public LogInvalidRequestMiddleware(RequestDelegate next, ILogger<LogInvalidRequestMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task Invoke(HttpContext context)
        {
            string requestBodyText = null;
            string responseBodyTxt = null;
            var originalRequestBody = context.Request.Body;

            using (MemoryStream requestBodyStream = new MemoryStream())
            {
                // request
                await context.Request.Body.CopyToAsync(requestBodyStream);
                requestBodyStream.Seek(0, SeekOrigin.Begin);
                requestBodyText = new StreamReader(requestBodyStream).ReadToEnd();
                requestBodyStream.Seek(0, SeekOrigin.Begin);
                context.Request.Body = requestBodyStream;

                // response
                Stream bodyStream = context.Response.Body;
                using (MemoryStream responseBodyStream = new MemoryStream())
                {
                    context.Response.Body = responseBodyStream;

                    await _next(context);
                    context.Request.Body = originalRequestBody;

                    responseBodyStream.Seek(0, SeekOrigin.Begin);
                    responseBodyTxt = new StreamReader(responseBodyStream).ReadToEnd();
                    responseBodyStream.Seek(0, SeekOrigin.Begin);
                    await responseBodyStream.CopyToAsync(bodyStream);

                    if (context.Response.StatusCode != StatusCodes.Status200OK)
                    {
                        _logger.LogError($"Status: {context.Response.StatusCode} for request: {requestBodyText} resulting in response: {responseBodyTxt}");
                    }
                }
            }
        }

        [Obsolete("Code moved to Invoke method - to use usings", true)]
        private async Task<string> GetBodyFromRequest(HttpContext context)
        {
            var requestBodyStream = new MemoryStream();

            await context.Request.Body.CopyToAsync(requestBodyStream);
            requestBodyStream.Seek(0, SeekOrigin.Begin);

            var requestBodyText = new StreamReader(requestBodyStream).ReadToEnd();

            requestBodyStream.Seek(0, SeekOrigin.Begin);
            context.Request.Body = requestBodyStream;

            return requestBodyText;
        }
    }
}
