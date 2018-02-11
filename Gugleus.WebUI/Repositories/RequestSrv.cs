using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Gugleus.Core.Domain.Requests;
using Gugleus.Core.Domain;
using Microsoft.Extensions.Configuration;
using Gugleus.Core.Repositories;

namespace Gugleus.WebUI.Repositories
{
    public class RequestSrv : IRequestSrv
    {
        private readonly AppDbContext _ctx;
        private readonly IConfiguration _config;

        public RequestSrv(AppDbContext ctx, IConfiguration config)
        {
            _ctx = ctx;
            _config = config;
        }

        public async Task<List<Request>> GetAllAsync(EnvType envType)
        {
            IRequestRepository repo  = PrepareRepository(envType);
            return await repo.GetAllAsync();
        }

        public async Task<Request> GetRequestByIdAsync(EnvType envType, long requestId)
        {
            IRequestRepository repo = PrepareRepository(envType);
            return await repo.GetRequestByIdAsync(requestId);
        }

        private RequestRepository PrepareRepository(EnvType envType)
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
            return new RequestRepository(_ctx, _config.GetConnectionString(csName));
        }
    }
}
