using Newtonsoft.Json;

namespace Gugleus.Core.Services
{
    public class UtilsService : IUtilsService
    {
        public T DeserializeFromJson<T>(string json)
        {
            return JsonConvert.DeserializeObject<T>(json);
        }

        public string SerializeToJson(object obj)
        {
            return JsonConvert.SerializeObject(obj);
        }
    }
}
