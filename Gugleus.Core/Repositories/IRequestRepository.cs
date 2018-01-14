using Gugleus.Core.Domain;
using Gugleus.Core.Domain.Requests;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Gugleus.Core.Repositories
{
    public interface IRequestRepository
    {
        Task<long> AddRequestAsync(Request request);

        Task<Request> GetRequestWithQueueAsync(long id, string requestType);

        Task<List<WsClient>> GetWsClientsAsync();
        Task<List<RequestStat>> GetStatsByDate(DateTime from, DateTime to);
    }
}
