using Gugleus.Core.Domain;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Gugleus.Core.Services
{
    public interface ICacheService
    {
        Task<List<WsClient>> GetWsClientsAsync();
    }
}
