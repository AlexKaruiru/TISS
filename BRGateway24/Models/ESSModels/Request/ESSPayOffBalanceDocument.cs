
using System.Xml.Serialization;
using static BRGateway24.Helpers.ESS.Dto.LoanDTO;

namespace BRGateway24.Models.ESSModels.Request
{
    // Pay Off Balance Request
    public class ESSPayOffBalanceDocument
    {
        // Employee Identification
        [XmlElement("CheckNumber")]
        [ColumnName("EMP_ID")]
        public long CheckNumber { get; set; }

        // Loan Information
        [XmlElement("LoanNumber")]
        [ColumnName("LOAN_NO")]
        public string LoanNumber { get; set; }

        // Employee Details
        [XmlElement("FirstName")]
        [ColumnName("FIRST_NAME")]
        public string FirstName { get; set; }

        [XmlElement("MiddleName")]
        [ColumnName("MIDDLE_NAME")]
        public string MiddleName { get; set; }

        [XmlElement("LastName")]
        [ColumnName("LAST_NAME")]
        public string LastName { get; set; }

        // Employer Information
        [XmlElement("VoteCode")]
        [ColumnName("VOTE_CODE")]
        public string VoteCode { get; set; }

        [XmlElement("VoteName")]
        [ColumnName("VOTE_NAME")]
        public string VoteName { get; set; }

        // Deduction Details
        [XmlElement("DeductionAmount")]
        [ColumnName("DEDUCTION_AMOUNT")]
        public decimal DeductionAmount { get; set; }

        [XmlElement("DeductionCode")]
        [ColumnName("DEDUCTION_CODE")]
        public string DeductionCode { get; set; }

        [XmlElement("DeductionName")]
        [ColumnName("DEDUCTION_DESC")]
        public string DeductionName { get; set; }

        [XmlElement("DeductionBalance")]
        [ColumnName("OUTSTANDING_BALANCE")]
        public decimal DeductionBalance { get; set; }

        // Financial Service Provider
        [XmlElement("FSPCode")]
        [ColumnName("FSP_CODE")]
        public string FSPCode { get; set; }

        // Payment Configuration
        [XmlElement("PaymentOption")]
        [ColumnName("PAY_OPTION")]
        public string PaymentOption { get; set; }
    }

}
