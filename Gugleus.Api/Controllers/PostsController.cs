using Gugleus.Api.Middleware;
using Gugleus.Core.Dto;
using Gugleus.Core.Results;
using Gugleus.Core.Services;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.SwaggerGen;
using System;
using System.Threading.Tasks;

namespace Gugleus.Api.Controllers
{
    [Route("[controller]")]
    public class PostsController : BaseController
    {
        private readonly IPostService _postService;
        private readonly IValidationService _validationService;

        public PostsController(IPostService postService, IValidationService validationService)
        {
            _postService = postService;
            _validationService = validationService;
        }


        [HttpGet("")]
        [SwaggerResponse(200, Type = typeof(string))]
        public IActionResult Get()
        {
            return Ok($"Ping at {DateTime.Now}.");
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetPostStatus(long id)
        {
            RequestStatusDto requestStatus = await _postService.GetPostStatus(id);
            if (requestStatus != null)
            {
                return Ok(requestStatus);
            }

            return BadRequest($"Post with Id: '{id}' not found...");
        }

        [HttpPost]
        [ValidateModel]
        public async Task<IActionResult> Post([FromBody]PostDto newPost)
        {
            IActionResult result;

            MessageListResult validationResult = _validationService.ValidateNewPost(newPost);

            if (!validationResult.IsOk)
            {
                result = BadRequest(validationResult);
            }
            else
            {
                var addResult = await _postService.AddPost(newPost);

                result = Ok(addResult);
            }

            return result;
        }
    }
}
