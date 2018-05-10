using Gugleus.Core.Domain;
using Gugleus.Core.Domain.Requests;
using Gugleus.Core.Repositories;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Gugleus.WebUI.Repositories
{
    public class RequestSrv : IRequestSrv
    {
        private readonly AppDbContext _ctx;
        private readonly IConfiguration _config;
        private readonly ILogger<RequestSrv> _logger;

        public RequestSrv(AppDbContext ctx, IConfiguration config, ILogger<RequestSrv> logger)
        {
            _ctx = ctx;
            _config = config;
            _logger = logger;
        }

        public async Task<List<Request>> GetAllAsync(EnvType envType)
        {
            IRequestRepository repo = PrepareRepository(envType);
            return await repo.GetAllAsync();
        }

        public IQueryable<Request> GetAllQueryable(EnvType envType)
        {
            IRequestRepository repo = PrepareRepository(envType);
            try
            {
                return repo.GetAllQueryable();
            }
            catch (Exception ex)
            {
                string cs = repo.GetConnectionString();
                _logger.LogError("GetAllQueryableAsync Ex for cs: {0} -> {1}", cs, ex.ToString());
                throw ex;
            }
        }

        public async Task<Request> GetRequestByIdAsync(EnvType envType, long requestId)
        {
            IRequestRepository repo = PrepareRepository(envType);
            return await repo.GetRequestByIdAsync(requestId);
        }

        private RequestEfRepository PrepareRepository(EnvType envType)
        {
            string csName = null;
            switch (envType)
            {
                case EnvType.Dev:
                    csName = "csDev";
                    break;
                case EnvType.Rc:
                    csName = "csRc";
                    break;
                case EnvType.Prod:
                    csName = "csProd";
                    break;
            }
            return new RequestEfRepository(_ctx, _config.GetConnectionString(csName));
        }
    }
}
