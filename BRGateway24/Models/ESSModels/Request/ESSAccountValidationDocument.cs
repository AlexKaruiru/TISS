using System.Xml.Serialization;
using static BRGateway24.Helpers.ESS.Dto.LoanDTO;

namespace BRGateway24.Models.ESSModels.Request
{
    [Serializable]
    [XmlRoot("Document")]
    public class ESSAccountValidationDocument : ESSBaseDocument<ESSAccountValidationHeader, ESSAccountValidationBody>
    {
        // Document-specific extensions can be added here
        // (if you need any custom behavior for account validation)
    }

    public class ESSAccountValidationHeader : ESSBaseHeader
    {
        [XmlElement("ValidationType")]
        public string ValidationType { get; set; } = "BANK_ACCOUNT";
    }

    public class ESSAccountValidationBody
    {
        [XmlElement("AccountNumber")]
        [ColumnName("ACCOUNT_NUMBER")]
        public string AccountNumber { get; set; }

        [XmlElement("FirstName")]
        [ColumnName("FIRST_NAME")]
        public string FirstName { get; set; }

        [XmlElement("MiddleName")]
        [ColumnName("MIDDLE_NAME")]
        public string MiddleName { get; set; }

        [XmlElement("LastName")]
        [ColumnName("LAST_NAME")]
        public string LastName { get; set; }

        [XmlElement("BankCode")]
        [ColumnName("BANK_CODE")]
        public string BankCode { get; set; }

        [XmlElement("ValidationTimestamp")]
        [ColumnName("VALIDATION_TIME")]
        public DateTime ValidationTimestamp { get; set; } = DateTime.UtcNow;
    }
}
