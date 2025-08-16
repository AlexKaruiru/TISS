
using System.Xml.Serialization;
using static BRGateway24.Helpers.ESS.Dto.LoanDTO;

namespace BRGateway24.Models.ESSModels.Request
{
    // Employee Cancellation
    public class ESSEmployeeCancellationDocument
    {
        [XmlElement("ApplicationNumber")] 
        public string ApplicationNumber { get; set; }
        [XmlElement("Reason")]
        [ColumnName("CANCEL_REASON")] 
        public string CancellationReason { get; set; }
        [XmlElement("FSPReferenceNumber")] 
        public string FSPReferenceNumber { get; set; }
        [XmlElement("LoanNumber")] 
        public string LoanNumber { get; set; }
    }
}
