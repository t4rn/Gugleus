using Gugleus.Core.Domain;
using System;
using System.Threading.Tasks;

namespace Gugleus.Core.Repositories
{
    public class PostRepository : IPostRepository
    {
        public async Task<long> AddPost(Post post)
        {
            return await Task.FromResult(new Random().Next(1, int.MaxValue));
        }

        public async Task<Post> GetPost(long id)
        {
            Post post = new Post() { Id = id };
            return await Task.FromResult(post);
        }
    }
}
