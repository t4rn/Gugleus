using Gugleus.Core.Domain.Dictionaries;
using Gugleus.Core.Results;

namespace Gugleus.Core.Dto.Input
{
    public abstract class AbstractRequestDto
    {
        public abstract MessageListResult Validate();

        internal abstract RequestType.RequestTypeCode RequestType { get; }

        /// <summary>
        /// Route name to method returning request status
        /// </summary>
        internal abstract string RouteName { get; }

        /// <summary>
        /// Filles result with IsOk/Message
        /// </summary>
        protected virtual void PrepareResult(MessageListResult result)
        {
            if (!result.MessageList.MsgsExist)
            {
                result.IsOk = true;
            }
            else
            {
                result.Message = $"Validation errors: {result.MessageList.GetMessages(";")}";
            }
        }
    }
}
