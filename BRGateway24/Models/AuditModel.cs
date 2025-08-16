using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.ComponentModel.DataAnnotations;
using System.Security.Cryptography.Xml;

namespace BRGateway24.Models
{


    public class AuditModel { }

    public class AuditRequestModel
    {
        //Example: [ "dbo.t_transaction", "dbo.t_AccountTrx" ]
        public List<string> Tables { get; set; }

        ///ISO 8601 timestamp, e.g. "2025-05-01T00:00:00".
        public DateTime? StartTime { get; set; }

        //ISO 8601 timestamp, e.g. "2025-05-15T23:59:59".        
        public DateTime? EndTime { get; set; }

        /// <summary>
        /// Optional. Filter by action type:
        ///   "DL" = DELETE
        ///   "UP" = UPDATE
        /// If omitted or null, both DELETE and UPDATE events will be returned.
        /// </summary>
        public string Action { get; set; }

        /// <summary>
        /// Optional. Only return events where ServerPrincipalName matches exactly.
        /// Example: "DOMAIN\\Alice"
        /// If omitted, events by all principals are returned.
        /// </summary>
        public string? User { get; set; }
    }

    public class AuditResponseModel
    {
        public DateTime EventTime { get; set; }
        public string ServerPrincipalName { get; set; }
        public string DatabaseName { get; set; }
        public string SchemaName { get; set; }
        public string ObjectName { get; set; }
        public string ActionId { get; set; }    // "DL" or "UP"
        public string Statement { get; set; }
    }    
}
