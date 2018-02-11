using Gugleus.Core.Domain.Requests;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;

namespace Gugleus.WebUI.Repositories
{
    public class RequestRepository : IRequestRepository
    {
        private readonly AppDbContext _appDbContext;

        public RequestRepository(AppDbContext appDbContext)
        {
            _appDbContext = appDbContext;
        }

        public IEnumerable<Request> GetAll()
        {
            return _appDbContext.Requests.AsNoTracking()
                .Include(x => x.WsClient)
                .Include(x => x.Type)
                .Include(x => x.Queue)
                .Include(x => x.Queue.Status);
        }

        public Request GetRequestById(long requestId)
        {
            return _appDbContext.Requests.AsNoTracking()
                .Include(x => x.WsClient)
                .Include(x => x.Type)
                .Include(x => x.Queue)
                .Include(x => x.Queue.Status)
                .FirstOrDefault(x => x.Id == requestId);
        }
    }
}
