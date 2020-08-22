using System.Collections.Generic;

namespace SnsTestReceiverApi.Models
{
    public class SnsMessage
    {
        public string Type { get; set; }
        public string MessageId { get; set; }
        public string TopicArn { get; set; }
        public string Subject { get; set; }
        public string Message { get; set; }
        public Dictionary<string, MessageAttributeValue> MessageAttributes { get; set; }
            = new Dictionary<string, MessageAttributeValue>();
        public string Timestamp { get; set; }
        public string SignatureVersion { get; set; }
        public string Signature { get; set; }
        public string SigningCertUrl { get; set; }
        public string UnsubscribeUrl { get; set; }
    }
}