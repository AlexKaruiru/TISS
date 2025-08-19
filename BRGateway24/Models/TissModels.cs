using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;

namespace BRGateway24.Models
{
    public class TissApiHeaders
    {
        [Required]
        public string Authorization { get; set; }

        [Required]
        public string Sender { get; set; }

        public string Consumer { get; set; }

        [Required]
        public string MsgId { get; set; }

        // For POST messages only
        public string ContentType { get; set; } = "application/xml";
        public string PayloadType { get; set; } = "XML";

        public string Currency { get; set; } = "TZS";

        public string ToJson()
        {
            return JsonConvert.SerializeObject(this);
        }

        public static TissApiHeaders CreateNew(
            string authorization,
            string sender,
            string consumer,
            string contentType = "application/xml",
            string payloadType = "XML",
            string currency = "TZS")
        {
            return new TissApiHeaders
            {
                Authorization = authorization,
                Sender = sender,
                Consumer = consumer,
                ContentType = contentType,
                PayloadType = payloadType,
                Currency = currency,
                MsgId = $"MSG_{Guid.NewGuid()}"
            };
        }
    }


    public class TissSendMessageRequest
    {
        [Required]
        public string MessageType { get; set; } // pacs.008, pacs.009, etc.

        [Required]
        public string PayloadXML { get; set; }

        public string Reference { get; set; }
    }

    public class TissApiRequest
    {
        public long RequestID { get; set; }
        public string MessageID { get; set; }
        public string Endpoint { get; set; }
        public string Method { get; set; }
        public string Headers { get; set; } // JSON serialized headers
        public string RequestBody { get; set; }
        public DateTime RequestTime { get; set; }
        public string ParticipantID { get; set; }
    }

    public class TissApiResponse
    {
        public long ResponseID { get; set; }
        public long RequestID { get; set; }
        public int StatusCode { get; set; }
        public string ResponseBody { get; set; }
        public DateTime ResponseTime { get; set; }
        public string ErrorDetails { get; set; }
    }

    public class InternalAuth
    {
        [Required]
        public string Token { get; set; }  
    }
}