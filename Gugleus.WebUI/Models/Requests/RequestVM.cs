using Gugleus.Core.Domain;
using Gugleus.Core.Domain.Dictionaries;
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


        /// <summary>
        /// Color for background on Request List table
        /// </summary>
        public string BackgroundColor
        {
            get
            {
                // if Error -> pale red
                return Queue?.Status == RequestStatus.RequestStatusCode.ERR.ToString() ?
                    "#ffb3b3" : "";
            }
        }

        public string InputShort(int chars)
        {
            if (!string.IsNullOrWhiteSpace(Input))
                return Input.Shorten(chars);

            return string.Empty;
        }

        public string OutputShort(int chars)
        {
            string retVal = string.Empty;
            if (Queue?.Status == RequestStatus.RequestStatusCode.ERR.ToString() &&
                !string.IsNullOrWhiteSpace(Queue?.ErrorMsg))
            {
                // on Error status show Error
                retVal = Queue.ErrorMsg.Shorten(chars);
            }
            else if (!string.IsNullOrWhiteSpace(Output))
            {
                // on other statuses show Output
                retVal = Output.Shorten(chars);
            }

            //if (!string.IsNullOrWhiteSpace(Output))
            //    return Output.Shorten(chars);

            return retVal;
        }
    }
}
