using System.Xml.Serialization;
using static BRGateway24.Helpers.ESS.Dto.LoanDTO;

namespace BRGateway24.Models.ESSModels.Request
{
    [XmlRoot("Document")]
    public class ESSLoanStatusDocument
    {
        [XmlElement("Data")]
        public ESSBaseData<ESSLoanStatusHeader, ESSLoanStatusBody> Data { get; set; }

        [XmlElement("Signature")]
        public string Signature { get; set; }
    }

    public class ESSLoanStatusHeader : ESSBaseHeader
    {
        [XmlElement("ResponseTimestamp")]
        public DateTime ResponseTimestamp { get; set; }
    }

    public class ESSLoanStatusBody
    {
        [XmlElement("ResponseCode")]
        [ColumnName("STATUS_CODE")]
        public int ResponseCode { get; set; }

        [XmlElement("Description")]
        [ColumnName("STATUS_MESSAGE")]
        public string Description { get; set; }

        [XmlElement("LoanNumber")]
        [ColumnName("LOAN_REFERENCE")]
        public string LoanNumber { get; set; }

        [XmlElement("LastUpdated")]
        [ColumnName("LAST_UPDATED")]
        public DateTime LastUpdated { get; set; }
    }
}
