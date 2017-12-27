namespace Gugleus.Core.Services
{
    public interface IUtilsService
    {
        string SerializeToJson(object obj);
        T DeserializeFromJson<T>(string json);
    }
}
