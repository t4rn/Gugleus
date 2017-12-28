namespace Gugleus.Core.Dto.Output
{
    public class RequestResponseDto<T>
    {
        public long Id { get; set; }
        public string Status { get; set; }
        public string Error { get; set; }
        public T Info { get; set; }
    }
}
