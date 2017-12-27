namespace Gugleus.Core.Dto
{
    public class RequestStatusDto
    {
        /// <summary>
        /// Request Id 
        /// </summary>
        public long Id { get; set; }
        public string Status { get; set; }
        public string Url { get; set; }
        public string Error { get; set; }
    }
}
