namespace Gugleus.Core.Dto.Output
{
    public class IdResultDto<T> : ResultDto
    {
        public string Url { get; set; }
        public T Id { get; set; }
    }
}
