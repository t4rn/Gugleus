using System.Threading.Tasks;

namespace Gugleus.Core.Repositories
{
    public interface IPostRepository
    {
        Task<long> AddPost(string post);

        Task<string> GetPost(long id);
    }
}
