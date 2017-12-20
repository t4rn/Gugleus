using Gugleus.Core.Auth;

namespace Gugleus.Api.Models
{
    public abstract class GenericRequest<T>
    {
        public Head Head { get; set; }
        public T Data { get; set; }
    }
}
