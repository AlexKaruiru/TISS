using System.Xml.Serialization;
using static BRGateway24.Helpers.ESS.Dto.LoanDTO;

namespace BRGateway24.Models.ESSModels.Request
{
    [Serializable]
    [XmlRoot("Document")]
    public class ESSDeductionStopDocument : ESSBaseDocument<ESSDeductionStopHeader, ESSDeductionStopBody>
    {
        // Add any deduction-stop-specific document methods here
        // Example: Custom validation for stop requests
    }

    public class ESSDeductionStopHeader : ESSBaseHeader
    {
        [XmlElement("StopEffectiveDate")]
        public DateTime EffectiveDate { get; set; } = DateTime.UtcNow.Date;

        [XmlElement("StopReasonCode")]
        public string ReasonCode { get; set; }
    }

    public class ESSDeductionStopBody
    {
        [XmlElement("ApplicationNumber")]
        [ColumnName("APP_NO")]
        public string ApplicationNumber { get; set; }

        [XmlElement("LoanNumber")]
        [ColumnName("LOAN_NO")]
        public string LoanNumber { get; set; }

        [XmlElement("CheckNumber")]
        [ColumnName("EMP_ID")]
        public long CheckNumber { get; set; }

        [XmlElement("DeductionCode")]
        [ColumnName("DEDUCT_CODE")]
        public string DeductionCode { get; set; }

        [XmlElement("DeductionDescription")]
        [ColumnName("DEDUCT_DESC")]
        public string DeductionDescription { get; set; }

        [XmlElement("BalanceAmount")]
        [ColumnName("OUTSTANDING_BAL")]
        public decimal BalanceAmount { get; set; }

        [XmlElement("DeductionAmount")]
        [ColumnName("MONTHLY_DEDUCTION")]
        public decimal DeductionAmount { get; set; }

        [XmlElement("StopReason")]
        [ColumnName("STOP_REASON")]
        public string StopReason { get; set; }

        [XmlElement("StopDate")]
        [ColumnName("STOP_DATE")]
        public DateTime StopDate { get; set; } = DateTime.UtcNow;

        [XmlElement("ProcessedBy")]
        [ColumnName("PROCESSED_BY")]
        public string ProcessedBy { get; set; }

        [XmlElement("AuthorizationCode")]
        [ColumnName("AUTH_CODE")]
        public string AuthorizationCode { get; set; }
    }
}
