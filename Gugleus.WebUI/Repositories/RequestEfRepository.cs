using Gugleus.Core.Domain.Requests;
using Gugleus.Core.Repositories;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Gugleus.Core.Domain;
using System;

namespace Gugleus.WebUI.Repositories
{
    public class RequestEfRepository : IRequestRepository
    {
        private readonly AppDbContext _appDbContext;

        public RequestEfRepository(AppDbContext appDbContext, string connectionString)
        {
            _appDbContext = appDbContext;
            _appDbContext.SetConnectionString(connectionString);
        }

        public Task<long> AddRequestAsync(Request request)
        {
            throw new NotImplementedException();
        }

        public async Task<List<Request>> GetAllWithPaginationAsync(int page = 0, int pageSize = 0)
        {
            IQueryable<Request> linqQuery = GetAllLinqQuery();

            if (page > 0 && pageSize > 0)
            {
                linqQuery = linqQuery.OrderBy(r => r.Id)
                    .Skip((page - 1) * pageSize).Take(pageSize);
            }

            return await linqQuery.ToListAsync();
        }

        public async Task<List<Request>> GetAllAsync()
        {
            return await GetAllLinqQuery().ToListAsync();
        }

        public async Task<Request> GetRequestByIdAsync(long requestId)
        {
            return await _appDbContext.Requests.AsNoTracking()
                .Include(x => x.WsClient)
                .Include(x => x.Type)
                .Include(x => x.Queue)
                .Include(x => x.Queue.Status)
                .FirstOrDefaultAsync(x => x.Id == requestId);
        }

        public Task<Request> GetRequestWithQueueAsync(long id, string requestType)
        {
            throw new NotImplementedException();
        }

        public Task<List<RequestStat>> GetStatsByDate(DateTime from, DateTime to)
        {
            throw new NotImplementedException();
        }

        public Task<List<WsClient>> GetWsClientsAsync()
        {
            throw new NotImplementedException();
        }

        public IQueryable<Request> GetAllQueryable()
        {
            return GetAllLinqQuery();
        }

        private IQueryable<Request> GetAllLinqQuery()
        {
            return _appDbContext.Requests.AsNoTracking()
                .Include(x => x.WsClient)
                .Include(x => x.Type)
                .Include(x => x.Queue)
                .Include(x => x.Queue.Status);
        }
    }
}
