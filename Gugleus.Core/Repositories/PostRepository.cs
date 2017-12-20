using System;
using System.Threading.Tasks;

namespace Gugleus.Core.Repositories
{
    public class PostRepository : IPostRepository
    {
        public async Task<long> AddPost(string post)
        {
            return await Task.FromResult(new Random().Next(1, int.MaxValue));
        }

        public async Task<string> GetPost(long id)
        {
            return await Task.FromResult($"Your post of Id: {id} is still in waiting queue...");
        }
    }
}
