using Gugleus.Core.Domain;
using Gugleus.Core.Extensions;
using System;

namespace Gugleus.WebUI.Models.Requests
{
    public class RequestVM
    {
        public long Id { get; set; }
        public string WsClient { get; set; }
        public string RequestType { get; set; }
        public string Input { get; set; }
        public string Output { get; set; }
        public DateTime AddDate { get; set; }
        public DateTime OutputDate { get; set; }
        public RequestQueueVM Queue { get; set; }
        public EnvType? Env { get; internal set; }

        public string InputShort(int chars)
        {
            if (!string.IsNullOrWhiteSpace(Input))
                return Input.Shorten(chars);

            return string.Empty;
        }

        public string OutputShort(int chars)
        {
            if (!string.IsNullOrWhiteSpace(Output))
                return Output.Shorten(chars);

            return string.Empty;
        }
    }
}
