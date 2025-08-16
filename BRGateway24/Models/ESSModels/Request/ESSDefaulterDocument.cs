using System.Xml.Serialization;
using static BRGateway24.Helpers.ESS.Dto.LoanDTO;

namespace BRGateway24.Models.ESSModels.Request
{
    [Serializable]
    [XmlRoot("Document")]
    public class ESSDefaulterDocument : ESSBaseDocument<ESSDefaulterHeader, ESSDefaulterBody>
    {
        // Optional: Add any defaulter-specific document methods here
        // Example: Custom validation for defaulter records
    }

    public class ESSDefaulterHeader : ESSBaseHeader
    {
        [XmlElement("DaysDelinquent")]
        public int DaysDelinquent { get; set; }

        [XmlElement("DefaultCategory")]
        public string DefaultCategory { get; set; } // "MILD", "MODERATE", "SEVERE"

        [XmlElement("LastPaymentDate")]
        public DateTime? LastPaymentDate { get; set; }
    }

    public class ESSDefaulterBody
    {
        [XmlElement("CheckNumber")]
        [ColumnName("EMP_ID")]
        public long CheckNumber { get; set; }

        [XmlElement("LoanNumber")]
        [ColumnName("LOAN_NO")]
        public string LoanNumber { get; set; }

        [XmlElement("FSPCode")]
        [ColumnName("FSP_CODE")]
        public string FSPCode { get; set; }

        [XmlElement("LastPayDate")]
        [ColumnName("LAST_PAY_DATE")]
        public DateTime LastPayDate { get; set; }

        [XmlElement("EmployeeStatus")]
        [ColumnName("EMP_STATUS")]
        public string EmployeeStatus { get; set; } // "ACTIVE", "INACTIVE", "SUSPENDED"

        [XmlElement("Workstation")]
        [ColumnName("WORKSTATION")]
        public string Workstation { get; set; }

        [XmlElement("PhysicalAddress")]
        [ColumnName("PHYSICAL_ADDR")]
        public string PhysicalAddress { get; set; }

        [XmlElement("TelephoneNumber")]
        [ColumnName("PHONE_NUMBER")]
        public string TelephoneNumber { get; set; }

        [XmlElement("EmailAddress")]
        [ColumnName("EMAIL")]
        public string EmailAddress { get; set; }

        [XmlElement("MobileNumber")]
        [ColumnName("MOBILE_NUMBER")]
        public string MobileNumber { get; set; }

        [XmlElement("ContactPerson")]
        [ColumnName("CONTACT_PERSON")]
        public string ContactPerson { get; set; }

        [XmlElement("Institution")]
        [ColumnName("INSTITUTION_NAME")]
        public string Institution { get; set; }

        [XmlElement("OutstandingBalance")]
        [ColumnName("OUTSTANDING_BAL")]
        public decimal OutstandingBalance { get; set; }

        [XmlElement("DefaultAmount")]
        [ColumnName("DEFAULT_AMOUNT")]
        public decimal DefaultAmount { get; set; }

        [XmlElement("DefaultDate")]
        [ColumnName("DEFAULT_DATE")]
        public DateTime DefaultDate { get; set; } = DateTime.UtcNow;

        [XmlElement("RecoveryStatus")]
        [ColumnName("RECOVERY_STATUS")]
        public string RecoveryStatus { get; set; } // "NEW", "IN_PROGRESS", "SETTLED"
    }
}
