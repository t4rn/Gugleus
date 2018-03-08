using Gugleus.Core.Domain.Dictionaries;
using Gugleus.Core.Results;
using System.ComponentModel.DataAnnotations;

namespace Gugleus.Core.Dto.Input
{
    public class PostDto : AbstractRequestDto
    {
        [Required]
        public UserInfoDto User { get; set; }
        [Required]
        public string Content { get; set; }
        //public string Place { get; set; }
        public ImageDto Image { get; set; }

        internal override RequestType.RequestTypeCode RequestType => Domain.Dictionaries.RequestType.RequestTypeCode.ADDPOST;

        internal override string RouteName => "GetPostStatus";

        public override MessageListResult Validate()
        {
            MessageListResult result = new MessageListResult();

            // validating
            if (User == null)
            {
                result.MessageList.Add("Null User data.");
            }
            if (string.IsNullOrWhiteSpace(Content))
            {
                result.MessageList.Add("Null Content data.");
            }

            // preparing result
            PrepareResult(result);

            return result;
        }
    }
}
