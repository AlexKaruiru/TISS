using System.Xml.Serialization;
using static BRGateway24.Helpers.ESS.Dto.LoanDTO;

namespace BRGateway24.Models.ESSModels.Request
{
    // Disbursement Failure
    public class ESSDisbursementFailureDocument
    {
        [XmlElement("ApplicationNumber")] public string ApplicationNumber { get; set; }
        [XmlElement("Reason")]
        [ColumnName("FAILURE_REASON")] 
        public string FailureReason { get; set; }
    }

}
