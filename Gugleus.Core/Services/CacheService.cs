using Gugleus.Core.Domain;
using Gugleus.Core.Repositories;
using Microsoft.Extensions.Caching.Memory;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Gugleus.Core.Services
{
    public class CacheService : ICacheService
    {
        private readonly IMemoryCache _cache;
        private readonly IRequestRepository _requestRepository;

        private enum CacheKey { WsClient }

        public CacheService(IMemoryCache cache, IRequestRepository requestRepository)
        {
            _cache = cache;
            _requestRepository = requestRepository;
        }

        public async Task<List<WsClient>> GetWsClientsAsync()
        {
            List<WsClient> wsClients;

            if (!_cache.TryGetValue(CacheKey.WsClient, out wsClients))
            {
                wsClients = await _requestRepository.GetWsClientsAsync();

                var cacheEntryOptions = new MemoryCacheEntryOptions()
                    .SetSlidingExpiration(TimeSpan.FromHours(1));

                // Save data in cache.
                _cache.Set(CacheKey.WsClient, wsClients, cacheEntryOptions);
            }

            return wsClients;
        }
    }
}
