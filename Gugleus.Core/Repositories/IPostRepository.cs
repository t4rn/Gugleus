using Gugleus.Core.Domain;
using System.Threading.Tasks;

namespace Gugleus.Core.Repositories
{
    public interface IPostRepository
    {
        Task<long> AddPost(Post post);

        Task<Post> GetPost(long id);
    }
}
