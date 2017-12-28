using AutoMapper;
using Gugleus.Api.Middleware;
using Gugleus.Core.Domain;
using Gugleus.Core.Dto.Input;
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
        private readonly IMapper _mapper;

        public PostsController(IRequestService requestService, IMapper mapper)
        {
            _requestService = requestService;
            _mapper = mapper;
        }

        [HttpGet("")]
        [SwaggerResponse(200, Type = typeof(string))]
        public IActionResult Ping()
        {
            return Ok($"Ping at {DateTime.Now}.");
        }

        [HttpGet("{id}")]
        [SwaggerResponse(200, Type = typeof(RequestResponseDto<GoogleInfo>))]
        [SwaggerResponse(400, Type = typeof(string))]
        [SwaggerResponse(500, Type = typeof(RequestResponseDto<GoogleInfo>))]
        public async Task<IActionResult> GetPostStatus(long id)
        {
            IActionResult result = await GetRequestResponse<GoogleInfo>(id, DictionaryItem.RequestType.ADDPOST);
            return result;
        }

        [HttpPost]
        [ValidateModel]
        [SwaggerResponse(200, Type = typeof(IdResultDto<long>))]
        [SwaggerResponse(400, Type = typeof(ResultDto))]
        [SwaggerResponse(500, Type = typeof(IdResultDto<long>))]
        public async Task<IActionResult> AddPost([FromBody]PostDto postDto)
        {
            IActionResult result = await ProcessRequestAsync(postDto);
            return result;
        }


        [HttpGet("details/{id}")]
        [ValidateModel]
        [SwaggerResponse(200, Type = typeof(RequestResponseDto<ActivityInfo>))]
        [SwaggerResponse(400, Type = typeof(string))]
        [SwaggerResponse(500, Type = typeof(RequestResponseDto<ActivityInfo>))]
        public async Task<IActionResult> GetPostDetails(long id)
        {
            IActionResult result = await GetRequestResponse<ActivityInfo>(id, DictionaryItem.RequestType.GETINFO);
            return result;
        }

        [HttpPost("details")]
        [ValidateModel]
        [SwaggerResponse(200, Type = typeof(IdResultDto<long>))]
        [SwaggerResponse(400, Type = typeof(ResultDto))]
        [SwaggerResponse(500, Type = typeof(IdResultDto<long>))]
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
            {
                if (string.IsNullOrWhiteSpace(requestStatus.Error))
                {
                    result = Ok(requestStatus);
                }
                else
                {
                    result = InternalServerError(requestStatus);
                }
            }
            else
                result = BadRequest($"Post with Id: '{id}' not found...");

            return result;
        }

        private async Task<IActionResult> ProcessRequestAsync<T>(T requestDto) where T : AbstractRequestDto
        {
            IActionResult result;

            if (requestDto == null) result = BadRequest(new ResultDto { Message = "Null input." });
            else
            {
                // validating input
                MessageListResult validationResult = requestDto.Validate();

                if (!validationResult.IsOk)
                {
                    ResultDto badValidationResult = _mapper.Map<ResultDto>(validationResult);
                    result = BadRequest(badValidationResult);
                }
                else
                {
                    // adding request to queue
                    IdResultDto<long> addResult = await _requestService.AddRequest(requestDto);

                    if (addResult.IsOk)
                        result = Ok(addResult);
                    else
                        result = InternalServerError(addResult);
                }
            }

            return result;
        }
    }
}
