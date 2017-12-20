using Gugleus.Api.ActionResults;
using Gugleus.Core.Repositories;
using Gugleus.Core.Results;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gugleus.Api.Controllers
{
    [Route("api/[controller]")]
    public class PlusController : Controller
    {
        private readonly IPostRepository _postRepository;

        public PlusController(IPostRepository postRepository)
        {
            _postRepository = postRepository;
        }


        [HttpGet("")]
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
            long id = await _postRepository.AddPost(newPost);
            Result result = new Result() { IsOk = true, Message = $"Successfully added post with Id: {id}." };

            return Ok(result);
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
