using AutoMapper;
using Gugleus.Core.Domain;
using Gugleus.Core.Domain.Dictionaries;
using Gugleus.Core.Dto.Input;
using Gugleus.Core.Dto.Output;
using Gugleus.Core.Results;
using Gugleus.Core.Services;
using Gugleus.GoogleCore;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Swashbuckle.AspNetCore.SwaggerGen;
using System;
using System.Globalization;
using System.IO;
using System.Linq;
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
        private readonly IConfiguration _configuration;
        private readonly string _ip;

        public PostsController(IRequestService requestService, IMapper mapper, ILogger<PostsController> logger,
            IActionContextAccessor actionContextAccessor, ICacheService cacheService, IConfiguration configuration) : base(cacheService)
        {
            _requestService = requestService;
            _mapper = mapper;
            _logger = logger;
            _configuration = configuration;
            _ip = actionContextAccessor?.ActionContext?.HttpContext?.Connection?.RemoteIpAddress?.ToString();
        }

        [HttpGet("")]
        public IActionResult Ping()
        {
            _logger.LogDebug($"{LogDescription()} Ping START");
            return Ok($"Ping at {DateTime.Now} from {_ip}.");
        }

        [HttpGet("{id}", Name = "GetPostStatus")]
        [SwaggerResponse(200, Type = typeof(RequestResponseDto<GoogleInfo>))]
        [SwaggerResponse(400, Type = typeof(string))]
        [SwaggerResponse(500, Type = typeof(RequestResponseDto<GoogleInfo>))]
        public async Task<IActionResult> GetPostStatus(long id)
        {
            IActionResult result = await GetRequestResponseAsync<GoogleInfo>(id, RequestType.RequestTypeCode.ADDPOST);
            return result;
        }

        [HttpPost]
        //[ValidateModel]
        [SwaggerResponse(200, Type = typeof(IdResultDto<long>))]
        [SwaggerResponse(400, Type = typeof(ResultDto))]
        [SwaggerResponse(500, Type = typeof(IdResultDto<long>))]
        public async Task<IActionResult> AddPost([FromBody]PostDto postDto)
        {
            IActionResult result;
            try
            {
                result = await ProcessRequestAsync(postDto);
            }
            catch (Exception ex)
            {
                // input params logged in LogInvalidRequestMiddleware.cs
                _logger.LogError($"[{LogDescription()}] Ex: {ex}");
                result = InternalServerError(ex.Message);
            }

            return result;
        }


        [HttpGet("details/{id}", Name = "GetPostDetails")]
        //[ValidateModel]
        //[SwaggerResponse(200, Type = typeof(RequestResponseDto<ActivityInfo>))]
        [ProducesResponseType(typeof(RequestResponseDto<ActivityInfo>), 200)]
        [SwaggerResponse(400, Type = typeof(string))]
        [ProducesResponseType(typeof(ObjectResult), 500)]
        public async Task<IActionResult> GetPostDetails(long id)
        {
            IActionResult result = await GetRequestResponseAsync<ActivityInfo>(id, RequestType.RequestTypeCode.GETINFO);
            return result;
        }

        [HttpPost("details")]
        //[ValidateModel]
        [SwaggerResponse(200, Type = typeof(IdResultDto<long>))]
        [SwaggerResponse(400, Type = typeof(ResultDto))]
        [SwaggerResponse(500, Type = typeof(IdResultDto<long>))]
        public async Task<IActionResult> AddPostDetailsRequest([FromBody]RequestDetailsDto requestDetailsDto)
        {
            IActionResult result;
            try
            {
                result = await ProcessRequestAsync(requestDetailsDto);
            }
            catch (Exception ex)
            {
                _logger.LogError($"[{LogDescription()}] Ex: {ex}");
                result = InternalServerError(ex.Message);
            }

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

        [HttpGet("img/{guid}")]
        [SwaggerResponse(200)]
        [SwaggerResponse(400, Type = typeof(string))]
        [SwaggerResponse(500, Type = typeof(string))]
        public IActionResult GetImg(string guid)
        {
            try
            {
                string folderForSearch = _configuration["img-dir"];
                var files = Directory.GetFiles(folderForSearch, $"{guid}.*");
                if (files.Any())
                {
                    return PhysicalFile(files.First(), "image/jpg");
                }
                else
                {
                    return BadRequest($"No file '{guid}' found...");
                }
            }
            catch (Exception ex)
            {
                return InternalServerError(ex.GetBaseException().Message);
            }
        }


        private async Task<IActionResult> GetRequestResponseAsync<T>(long id, RequestType.RequestTypeCode requestType) where T : class
        {
            IActionResult result;
            try
            {
                ObjResult<RequestResponseDto<T>> requestStatusResult =
                    await _requestService.GetRequestResponseAsync<T>(id, requestType);

                if (requestStatusResult.IsOk)
                {
                    _logger.LogDebug("{0} Ok for Id: '{1}' type: '{2}' -> {3}",
                        LogDescription(), id, requestType, requestStatusResult.Object.Status);
                    result = Ok(requestStatusResult.Object);
                }
                else
                {
                    _logger.LogError($"{LogDescription()} Request with Id: '{id}' and type '{requestType}' not found");
                    result = BadRequest($"Request with Id: '{id}' not found...");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError("[{0}] Ex for Id: '{1}' type: '{2}': {3}", nameof(GetRequestResponseAsync), id, requestType, ex);
                result = InternalServerError(ex.Message);
            }
            return result;
        }

        private async Task<IActionResult> ProcessRequestAsync<T>(T requestDto) where T : AbstractRequestDto
        {
            IActionResult result;

            string typeForLogs = typeof(T).Name;

            if (requestDto == null)
            {
                _logger.LogError("{0} Null input for '{1}'", LogDescription(), typeForLogs);
                result = BadRequest(new ResultDto { Message = "Null input." });
            }
            else
            {
                // validating input
                MessageListResult validationResult = requestDto.Validate();

                if (!validationResult.IsOk)
                {
                    _logger.LogError("{0} ValidErr for '{1}': {2}",
                        LogDescription(), typeForLogs, validationResult.Message);
                    ResultDto badValidationResult = _mapper.Map<ResultDto>(validationResult);
                    result = BadRequest(badValidationResult);
                }
                else
                {
                    // getting wsClient
                    WsClient wsClient = await GetWsClient();

                    // adding request to queue
                    IdResultDto<long> addResult = await _requestService.AddRequestAsync(requestDto, wsClient);

                    if (addResult.IsOk)
                    {
                        _logger.LogDebug("{0} Ok for: '{1}' -> Id: '{2}'",
                            LogDescription(), typeForLogs, addResult.Id);
                        result = Ok(addResult);
                    }
                    else
                    {
                        // when ID from DB <= 0
                        _logger.LogError("{0} Error for: '{1}' -> Message: '{2}'",
                            LogDescription(), typeForLogs, addResult.Message);
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
