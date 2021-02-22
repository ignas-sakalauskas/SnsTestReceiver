using System;
using System.Collections.Generic;

namespace SnsTestReceiver.Api.SqsPolling
{
    public class SqsSettings
    {
        public List<Uri> Urls { get; set; }
        public string Region { get; set; }
        // ReSharper disable once InconsistentNaming
        public Uri ServiceURL { get; set; }
    }
}