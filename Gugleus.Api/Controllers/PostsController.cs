using Gugleus.Core.Repositories;
using Gugleus.Core.Results;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.SwaggerGen;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gugleus.Api.Controllers
{
    [Route("api/[controller]")]
    public class PostsController : BaseController
    {
        private readonly IPostRepository _postRepository;

        public PostsController(IPostRepository postRepository)
        {
            _postRepository = postRepository;
        }


        [HttpGet("")]
        [SwaggerResponse(200, Type = typeof(string))]
        public IActionResult Get()
        {
            return Ok($"Ping at {DateTime.Now}.");
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(long id)
        {
            var post = await _postRepository.GetPost(id);
            if (post != null)
            {
                return Ok(post);
            }

            return BadRequest($"Post with Id: '{id}' not found...");
        }

        [HttpPost]
        public async Task<IActionResult> Post([FromBody]string newPost)
        {
            IActionResult result;

            if (newPost == null)
            {
                result = BadRequest("Null input.");
            }
            else if (!ModelState.IsValid)
            {
                result = BadRequest(ModelState);
            }
            else
            {
                long id = await _postRepository.AddPost(newPost);
                var res = new Result() { IsOk = true, Message = $"Successfully added post with Id: {id}." };

                result = Ok(res);
            }

            return result;
        }

        [HttpPut("{id}")]
        public void Put(int id, [FromBody]string value)
        {
        }

        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
