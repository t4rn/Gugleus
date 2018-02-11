using Gugleus.Core.Domain;
using Gugleus.Core.Domain.Requests;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Gugleus.WebUI.Repositories
{
    public interface IRequestSrv
    {
        Task<List<Request>> GetAllAsync(EnvType envType);

        Task<Request> GetRequestByIdAsync(EnvType envType, long requestId);
    }
}
