using Gugleus.Core.Domain.Requests;
using System;
using System.Threading.Tasks;

namespace Gugleus.Core.Repositories
{
    public class RequestRepository : IRequestRepository
    {
        public async Task<long> AddRequest(Request request)
        {
            return await Task.FromResult(new Random().Next(1, int.MaxValue));
        }

        public async Task<Request> GetRequest(long id)
        {
            Request request = new Request() { Id = id };
            return await Task.FromResult(request);
        }
    }
}
