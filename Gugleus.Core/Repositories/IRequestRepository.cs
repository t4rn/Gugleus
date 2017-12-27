using Gugleus.Core.Domain.Requests;
using System.Threading.Tasks;

namespace Gugleus.Core.Repositories
{
    public interface IRequestRepository
    {
        Task<long> AddRequest(Request request);

        Task<Request> GetRequest(long id);
    }
}
