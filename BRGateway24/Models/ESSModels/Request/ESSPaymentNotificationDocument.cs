
using System.Xml.Serialization;
using static BRGateway24.Helpers.ESS.Dto.LoanDTO;

namespace BRGateway24.Models.ESSModels.Request
{
    // Payment Notification
    public class ESSPaymentNotificationDocument
    {
        // Loan Identification (1-2)
        [XmlElement("ApplicationNumber")]
        [ColumnName("APP_NO")]
        public string ApplicationNumber { get; set; }

        [XmlElement("LoanNumber")]
        [ColumnName("LOAN_NO")]
        public string LoanNumber { get; set; }

        // Payment Reference (3-4)
        [XmlElement("PaymentReferenceNumber")]
        [ColumnName("PAYMENT_REF_NO")]
        public string PaymentReferenceNumber { get; set; }

        [XmlElement("FSPReferenceNumber")]
        [ColumnName("FSP_REF_NO")]
        public string FSPReferenceNumber { get; set; }

        // Payment Details (5-8)
        [XmlElement("TotalPayoffAmount")]
        [ColumnName("TOTAL_PAYMENT")]
        public decimal TotalPayoffAmount { get; set; }

        [XmlElement("PaymentDate")]
        [ColumnName("PAYMENT_DATE")]
        public DateTime PaymentDate { get; set; }

        [XmlElement("PaymentAdvice")]
        [ColumnName("PAYMENT_DETAILS")]
        public string PaymentAdvice { get; set; }

        [XmlElement("PaymentAdviceAttachment")]
        [ColumnName("PAYMENT_ADVICE")]
        public string PaymentAdviceAttachment { get; set; }
    }
}
