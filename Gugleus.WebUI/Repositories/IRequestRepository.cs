using Gugleus.Core.Domain.Requests;
using System.Collections.Generic;

namespace Gugleus.WebUI.Repositories
{
    public interface IRequestRepository
    {
        IEnumerable<Request> GetAll();
        Request GetRequestById(long requestId);
    }
}
