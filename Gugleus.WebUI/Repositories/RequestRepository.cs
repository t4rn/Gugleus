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
            return _appDbContext.Requests
                .Include(x => x.WsClient)
                .Include(x => x.Type);
        }

        public Request GetRequestById(long requestId)
        {
            return _appDbContext.Requests.FirstOrDefault(x => x.Id == requestId);
        }
    }
}
