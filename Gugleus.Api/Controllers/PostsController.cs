using AutoMapper;
using Gugleus.Api.Middleware;
using Gugleus.Core.Domain;
using Gugleus.Core.Dto.Input;
using Gugleus.Core.Dto.Output;
using Gugleus.Core.Results;
using Gugleus.Core.Services;
using Gugleus.GoogleCore;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Swashbuckle.AspNetCore.SwaggerGen;
using System;
using System.Globalization;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace Gugleus.Api.Controllers
{
    [Route("[controller]")]
    public class PostsController : BaseController
    {
        private readonly IRequestService _requestService;
        private readonly IMapper _mapper;
        private readonly ILogger<PostsController> _logger;
        private readonly string _ip;

        public PostsController(IRequestService requestService, IMapper mapper, ILogger<PostsController> logger,
            IHttpContextAccessor httpContextAccessor)
        {
            _requestService = requestService;
            _mapper = mapper;
            _logger = logger;
            _ip = httpContextAccessor?.HttpContext?.Connection?.RemoteIpAddress?.ToString();
        }

        [HttpGet("")]
        public IActionResult Ping()
        {
            _logger.LogDebug($"{LogDescription()} Ping START");
            return Ok($"Ping at {DateTime.Now} from {_ip}.");
        }

        [HttpGet("{id}")]
        [SwaggerResponse(200, Type = typeof(RequestResponseDto<GoogleInfo>))]
        [SwaggerResponse(400, Type = typeof(string))]
        [SwaggerResponse(500, Type = typeof(RequestResponseDto<GoogleInfo>))]
        public async Task<IActionResult> GetPostStatus(long id)
        {
            IActionResult result = await GetRequestResponseAsync<GoogleInfo>(id, DictionaryItem.RequestType.ADDPOST);
            return result;
        }

        [HttpPost]
        //[ValidateModel]
        [SwaggerResponse(200, Type = typeof(IdResultDto<long>))]
        [SwaggerResponse(400, Type = typeof(ResultDto))]
        [SwaggerResponse(500, Type = typeof(IdResultDto<long>))]
        public async Task<IActionResult> AddPost([FromBody]PostDto postDto)
        {
            IActionResult result = await ProcessRequestAsync(postDto);
            return result;
        }


        [HttpGet("details/{id}")]
        //[ValidateModel]
        [SwaggerResponse(200, Type = typeof(RequestResponseDto<ActivityInfo>))]
        [SwaggerResponse(400, Type = typeof(string))]
        [SwaggerResponse(500, Type = typeof(RequestResponseDto<ActivityInfo>))]
        public async Task<IActionResult> GetPostDetails(long id)
        {
            IActionResult result = await GetRequestResponseAsync<ActivityInfo>(id, DictionaryItem.RequestType.GETINFO);
            return result;
        }

        [HttpPost("details")]
        //[ValidateModel]
        [SwaggerResponse(200, Type = typeof(IdResultDto<long>))]
        [SwaggerResponse(400, Type = typeof(ResultDto))]
        [SwaggerResponse(500, Type = typeof(IdResultDto<long>))]
        public async Task<IActionResult> AddPostDetailsRequest([FromBody]RequestDetailsDto requestDetailsDto)
        {
            IActionResult result = await ProcessRequestAsync(requestDetailsDto);
            return result;
        }

        [HttpGet("stats/{from}/{to}")]
        [SwaggerResponse(200, Type = typeof(RequestSummaryDto<DateFilterDto>),
            Description = "Example: http://localhost:65508/posts/stats/20170101/20180131")]
        [SwaggerResponse(400, Type = typeof(string))]
        [SwaggerResponse(500, Type = typeof(string))]
        public async Task<IActionResult> GetStats(string from, string to)
        {
            IActionResult result;

            try
            {
                DateTime fromDate = DateTime.ParseExact(from, "yyyyMMdd", CultureInfo.InvariantCulture);
                DateTime toDate = DateTime.ParseExact(to, "yyyyMMdd", CultureInfo.InvariantCulture);

                if (fromDate <= toDate)
                {
                    RequestSummaryDto<DateFilterDto> statResult = await _requestService.GetStatsByDate(fromDate, toDate);
                    result = Ok(statResult);
                }
                else
                {
                    result = BadRequest($"From date: '{fromDate}' is later then To date: '{toDate}'.");
                }
            }
            catch (Exception ex)
            {
                result = InternalServerError(ex.Message);
            }

            return result;
        }



        private async Task<IActionResult> GetRequestResponseAsync<T>(long id, DictionaryItem.RequestType requestType) where T : class
        {
            IActionResult result;
            RequestResponseDto<T> requestStatus =
                await _requestService.GetRequestResponseAsync<T>(id, requestType);

            if (requestStatus != null)
            {
                if (string.IsNullOrWhiteSpace(requestStatus.Error))
                {
                    _logger.LogDebug("{0} Ok for Id: '{1}' type: '{2}' -> {3}",
                        LogDescription(), id, requestType, requestStatus.Status);
                    result = Ok(requestStatus);
                }
                else
                {
                    _logger.LogError("{0} Error for Id: '{1}' type: '{2}' -> {3}",
                        LogDescription(), id, requestType, requestStatus.Error);
                    result = InternalServerError(requestStatus);
                }
            }
            else
            {
                _logger.LogError($"{LogDescription()} Post with Id: '{id}' and type '{requestType}' not found");
                result = BadRequest($"Post with Id: '{id}' not found...");
            }

            return result;
        }

        private async Task<IActionResult> ProcessRequestAsync<T>(T requestDto) where T : AbstractRequestDto
        {
            IActionResult result;

            if (requestDto == null)
            {
                _logger.LogError("{0} Null input for '{1}'", LogDescription(), typeof(T));
                result = BadRequest(new ResultDto { Message = "Null input." });
            }
            else
            {
                // validating input
                MessageListResult validationResult = requestDto.Validate();

                if (!validationResult.IsOk)
                {
                    _logger.LogError("{0} ValidErr for '{1}': {2}",
                        LogDescription(), typeof(T), validationResult.Message);
                    ResultDto badValidationResult = _mapper.Map<ResultDto>(validationResult);
                    result = BadRequest(badValidationResult);
                }
                else
                {
                    // adding request to queue
                    IdResultDto<long> addResult = await _requestService.AddRequestAsync(requestDto);

                    if (addResult.IsOk)
                    {
                        _logger.LogDebug("{0} Ok for: '{1}' -> Id: '{2}'",
                            LogDescription(), typeof(T), addResult.Id);
                        result = Ok(addResult);
                    }
                    else
                    {
                        _logger.LogError("{0} Error for: '{1}' -> Message: '{2}'",
                            LogDescription(), typeof(T), addResult.Message);
                        result = InternalServerError(addResult);
                    }
                }
            }

            return result;
        }

        private string LogDescription([CallerMemberName] string methodName = null)
        {
            return $"[{methodName} - {_ip}]";
        }
    }
}
