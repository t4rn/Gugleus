using System;

namespace Gugleus.WebUI.Models
{
    public class ErrorViewModel
    {
        public string RequestId { get; set; }

        public bool ShowRequestId => !string.IsNullOrEmpty(RequestId);

        public string Path { get; set; }
        public string Exception { get; set; }
    }
}