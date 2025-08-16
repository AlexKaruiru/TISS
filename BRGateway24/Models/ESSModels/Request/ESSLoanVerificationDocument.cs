
using System.Xml.Serialization;
using static BRGateway24.Helpers.ESS.Dto.LoanDTO;

namespace BRGateway24.Models.ESSModels.Request
{
    // Loan Verification and Approval
    public class ESSLoanVerificationDocument
    {
        [XmlElement("ApplicationNumber")]
        [ColumnName("APP_NO")] 
        public string ApplicationNumber { get; set; }
        [XmlElement("Reason")]
        [ColumnName("REJECT_REASON")] 
        public string Reason { get; set; }
        [XmlElement("FSPReferenceNumber")]
        [ColumnName("FSP_REF")] 
        public string FSPReferenceNumber { get; set; }
        [XmlElement("Approval")]
        [ColumnName("APPROVAL_STATUS")] 
        public string ApprovalStatus { get; set; }
        [XmlElement("LoanNumber")]
        [ColumnName("LOAN_NO")] 
        public string LoanNumber { get; set; }
    }
}
