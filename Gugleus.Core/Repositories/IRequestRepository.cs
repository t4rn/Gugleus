using Gugleus.Core.Domain;
using Gugleus.Core.Domain.Requests;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;

namespace Gugleus.Core.Repositories
{
    public interface IRequestRepository
    {
        Task<long> AddRequestAsync(Request request);

        Task<Request> GetRequestWithQueueAsync(long id, string requestType);

        Task<List<WsClient>> GetWsClientsAsync();

        Task<List<RequestStat>> GetStatsByDate(DateTime from, DateTime to);

        Task<List<Request>> GetAllAsync();

        Task<Request> GetRequestByIdAsync(long requestId);

        IQueryable<Request> GetAllQueryable();

        string GetConnectionString();
    }
}
