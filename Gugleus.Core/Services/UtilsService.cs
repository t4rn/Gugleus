using Newtonsoft.Json;

namespace Gugleus.Core.Services
{
    public class UtilsService : IUtilsService
    {
        public string SerializeToJson(object obj)
        {
            return JsonConvert.SerializeObject(obj);
        }
    }
}
