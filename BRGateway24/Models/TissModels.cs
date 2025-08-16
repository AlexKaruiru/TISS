using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;

namespace BRGateway24.Models
{
    public class TissApiHeaders
    {
        [Required]
        public string Authorization { get; set; }

        [Required]
        public string Sender { get; set; } // Participant BIC

        public string Consumer { get; set; } = "TANZTZTX"; // Central Bank BIC

        [Required]
        public string MsgId { get; set; }

        // For POST messages only
        public string ContentType { get; set; }
        public string PayloadType { get; set; }

        // Move these methods inside the class
        public string ToJson()
        {
            return JsonConvert.SerializeObject(this);
        }

        public static TissApiHeaders FromJson(string json)
        {
            return JsonConvert.DeserializeObject<TissApiHeaders>(json);
        }
    }

    public class TissBusinessDateResponse
    {
        public string Currency { get; set; }
        public string BusinessDate { get; set; } // YYYYMMDD
    }

    public class TissTimetableEventResponse
    {
        public string EventCode { get; set; }
        public string EventName { get; set; }
        public string StartTime { get; set; } // HH:mm:ss
        public string EndTime { get; set; }
        public string Status { get; set; }
    }

    public class TissPendingTransaction
    {
        public string TransactionID { get; set; }
        public decimal Amount { get; set; }
        public string Currency { get; set; }
        public string CounterpartyBIC { get; set; }
        public string Status { get; set; }
        public string CreatedAt { get; set; }
    }

    public class TissAccountActivity
    {
        public long ActivityID { get; set; }
        public string AccountID { get; set; }
        public string ActivityType { get; set; }
        public decimal Amount { get; set; }
        public string Currency { get; set; }
        public decimal? BalanceAfter { get; set; }
        public string BookedAt { get; set; }
        public string Narrative { get; set; }
        public string Reference { get; set; }
    }

    public class TissMessageResponse
    {
        public string MessageID { get; set; }
        public string Status { get; set; }
        public string ResponseDetails { get; set; }
        public string ProcessedAt { get; set; }
    }

    public class TissSendMessageRequest
    {
        [Required]
        public string MessageType { get; set; } // pacs.008, pacs.009, etc.

        [Required]
        public string PayloadXML { get; set; }

        public string Reference { get; set; }
    }

    // Add these new models
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
} 