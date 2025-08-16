namespace BRGateway24.Models.ESSModels.Responses
{
    using System;
    using System.Data;
    using System.Xml.Serialization;
    using System.IO;

    [XmlRoot("Document")]
    public class LoanResponseDocument
    {
        [XmlElement("Data")]
        public LoanResponseData Data { get; set; }

        [XmlElement("Signature")]
        public string Signature { get; set; } = "Signature";
    }

    public class LoanResponseData
    {
        [XmlElement("Header")]
        public ResponseHeader Header { get; set; }

        [XmlElement("MessageDetails")]
        public ESSNewLoanResponse MessageDetails { get; set; }
    }

    public class ResponseHeader
    {
        [XmlElement("Sender")]
        public string Sender { get; set; } = "ESS_UTUMISHI";

        [XmlElement("Receiver")]
        public string Receiver { get; set; } = "FSPName";

        [XmlElement("FSPCode")]
        public string FSPCode { get; set; } = "A1001";

        [XmlElement("MsgId")]
        public string MessageId { get; set; } = Guid.NewGuid().ToString();

        [XmlElement("MessageType")]
        public string MessageType { get; set; } = "LOAN_CHARGES_RESPONSE";
    }

    public class ESSNewLoanResponse
    {
        [XmlElement("DesiredDeductibleAmount")]
        public decimal? DesiredDeductibleAmount { get; set; }

        [XmlElement("TotalInsurance")]
        public decimal TotalInsurance { get; set; }

        [XmlElement("TotalProcessingFees")]
        public decimal? TotalProcessingFees { get; set; }

        [XmlElement("TotalInterestRateAmount")]
        public decimal TotalInterestRateAmount { get; set; }

        [XmlElement("OtherCharges")]
        public decimal? OtherCharges { get; set; }

        [XmlElement("NetLoanAmount")]
        public decimal NetLoanAmount { get; set; }

        [XmlElement("TotalAmountToPay")]
        public decimal TotalAmountToPay { get; set; }

        [XmlElement("Tenure")]
        public int Tenure { get; set; }

        [XmlElement("EligibleAmount")]
        public decimal? EligibleAmount { get; set; }

        [XmlElement("MonthlyReturnAmount")]
        public decimal MonthlyReturnAmount { get; set; }
    }
   
}
