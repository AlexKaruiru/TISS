
using System.Xml.Serialization;
using static BRGateway24.Helpers.ESS.Dto.LoanDTO;

namespace BRGateway24.Models.ESSModels.Request
{
    // Partial Loan Repayment
    public class ESSPartialRepaymentDocument
    {
        // Employee Identification (1-3)
        [XmlElement("CheckNumber")]
        [ColumnName("EMP_ID")]
        public long CheckNumber { get; set; }

        [XmlElement("LoanNumber")]
        [ColumnName("LOAN_NO")]
        public string LoanNumber { get; set; }

        [XmlElement("FirstName")]
        [ColumnName("FIRST_NAME")]
        public string FirstName { get; set; }

        // Employee Details (4-7)
        [XmlElement("MiddleName")]
        [ColumnName("MIDDLE_NAME")]
        public string MiddleName { get; set; }

        [XmlElement("LastName")]
        [ColumnName("LAST_NAME")]
        public string LastName { get; set; }

        [XmlElement("VoteCode")]
        [ColumnName("VOTE_CODE")]
        public string VoteCode { get; set; }

        [XmlElement("VoteName")]
        [ColumnName("VOTE_NAME")]
        public string VoteName { get; set; }

        // Deduction Information (8-11)
        [XmlElement("DeductionAmount")]
        [ColumnName("DEDUCT_AMOUNT")]
        public decimal DeductionAmount { get; set; }

        [XmlElement("DeductionCode")]
        [ColumnName("DEDUCT_CODE")]
        public string DeductionCode { get; set; }

        [XmlElement("DeductionName")]
        [ColumnName("DEDUCT_DESC")]
        public string DeductionName { get; set; }

        [XmlElement("DeductionBalance")]
        [ColumnName("OUTSTANDING_BAL")]
        public decimal DeductionBalance { get; set; }

        // Financial Service Provider (12)
        [XmlElement("FSPCode")]
        [ColumnName("FSP_CODE")]
        public string FSPCode { get; set; }

        // Payment Details (13-15)
        [XmlElement("PaymentOption")]
        [ColumnName("PAY_OPTION")]
        public string PaymentOption { get; set; }

        [XmlElement("IntentionOfPartialPayment")]
        [ColumnName("PAYMENT_INTENT")]
        public string IntentionOfPartialPayment { get; set; }

        [XmlElement("AmountToPay")]
        [ColumnName("PAY_AMOUNT")]
        public decimal AmountToPay { get; set; }
    }

}
