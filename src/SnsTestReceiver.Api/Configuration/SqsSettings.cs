using System;
using System.Collections.Generic;

namespace SnsTestReceiver.Api.Configuration
{
    public class SqsSettings
    {
        public List<Uri> Urls { get; set; } = new List<Uri>();
        public string Region { get; set; }
        // ReSharper disable once InconsistentNaming
        public Uri ServiceURL { get; set; }
    }
}