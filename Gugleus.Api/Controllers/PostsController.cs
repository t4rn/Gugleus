using Gugleus.Api.Middleware;
using Gugleus.Core.Domain;
using Gugleus.Core.Dto;
using Gugleus.Core.Dto.Output;
using Gugleus.Core.Results;
using Gugleus.Core.Services;
using Gugleus.GoogleCore;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.SwaggerGen;
using System;
using System.Threading.Tasks;

namespace Gugleus.Api.Controllers
{
    [Route("[controller]")]
    public class PostsController : BaseController
    {
        private readonly IRequestService _requestService;
        private readonly IValidationService _validationService;

        public PostsController(IRequestService requestService, IValidationService validationService)
        {
            _requestService = requestService;
            _validationService = validationService;
        }

        [HttpGet("")]
        [SwaggerResponse(200, Type = typeof(string))]
        public IActionResult Ping()
        {
            return Ok($"Ping at {DateTime.Now}.");
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetPostStatus(long id)
        {
            IActionResult result = await GetRequestResponse<GoogleInfo>(id, DictionaryItem.RequestType.ADDPOST);
            return result;
        }

        [HttpPost]
        [ValidateModel]
        public async Task<IActionResult> AddPost([FromBody]PostDto postDto)
        {
            IActionResult result = await ProcessRequestAsync(postDto);
            return result;
        }


        [HttpGet("details/{id}")]
        [ValidateModel]
        public async Task<IActionResult> GetPostDetails(long id)
        {
            IActionResult result = await GetRequestResponse<ActivityInfo>(id, DictionaryItem.RequestType.GETINFO);
            return result;
        }

        [HttpPost("details")]
        [ValidateModel]
        public async Task<IActionResult> AddPostDetailsRequest([FromBody]RequestDetailsDto requestDetailsDto)
        {
            IActionResult result = await ProcessRequestAsync(requestDetailsDto);
            return result;
        }


        private async Task<IActionResult> GetRequestResponse<T>(long id, DictionaryItem.RequestType requestType) where T : class
        {
            IActionResult result;
            RequestResponseDto<T> requestStatus =
                await _requestService.GetRequestResponse<T>(id, requestType);

            if (requestStatus != null)
                result = Ok(requestStatus);
            else
                result = BadRequest($"Post with Id: '{id}' not found...");

            return result;
        }

        private async Task<IActionResult> ProcessRequestAsync<T>(T requestDto) where T : AbstractRequestDto
        {
            IActionResult result;

            if (requestDto == null) result = BadRequest("Null input.");
            else
            {
                // validating input
                MessageListResult validationResult = requestDto.Validate();

                if (!validationResult.IsOk)
                {
                    result = BadRequest(validationResult);
                }
                else
                {
                    // adding request to queue
                    IdResultDto<long> addResult = await _requestService.AddRequest(requestDto);

                    if (addResult.IsOk)
                    {
                        result = Ok(addResult);
                    }
                    else
                    {
                        result = InternalServerError(addResult);
                    }
                }
            }

            return result;
        }
    }
}
