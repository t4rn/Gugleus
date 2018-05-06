using Gugleus.Core.Domain;
using Gugleus.Core.Domain.Requests;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Gugleus.WebUI.Repositories
{
    public interface IRequestSrv
    {
        Task<List<Request>> GetAllAsync(EnvType envType);
        IQueryable<Request> GetAllQueryableAsync(EnvType envType);

        Task<Request> GetRequestByIdAsync(EnvType envType, long requestId);
    }
}
