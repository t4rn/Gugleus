namespace Gugleus.Core.Results
{
    public class ObjResult<T> : Result
    {
        public T Object { get; set; }
    }
}
