
using System.Xml.Serialization;
using static BRGateway24.Helpers.ESS.Dto.LoanDTO;

namespace BRGateway24.Models.ESSModels.Request
{
    // Monthly Deduction Record
    public class ESSMonthlyDeductionDocument
    {
        // Loan Identification (1-2)
        [XmlElement("ApplicationNumber")]
        [ColumnName("APP_NO")]
        public string ApplicationNumber { get; set; }

        [XmlElement("LoanNumber")]
        [ColumnName("LOAN_NO")]
        public string LoanNumber { get; set; }

        // Employee Identification (3-7)
        [XmlElement("CheckNumber")]
        [ColumnName("EMP_ID")]
        public long CheckNumber { get; set; }

        [XmlElement("FirstName")]
        [ColumnName("FIRST_NAME")]
        public string FirstName { get; set; }

        [XmlElement("MiddleName")]
        [ColumnName("MIDDLE_NAME")]
        public string MiddleName { get; set; }

        [XmlElement("LastName")]
        [ColumnName("LAST_NAME")]
        public string LastName { get; set; }

        [XmlElement("NationalId")]
        [ColumnName("NATIONAL_ID")]
        public string NationalId { get; set; }

        // Employer Information (8-12)
        [XmlElement("VoteCode")]
        [ColumnName("VOTE_CODE")]
        public string VoteCode { get; set; }

        [XmlElement("VoteName")]
        [ColumnName("EMPLOYER_NAME")]
        public string VoteName { get; set; }

        [XmlElement("DepartmentCode")]
        [ColumnName("DEPT_CODE")]
        public string DepartmentCode { get; set; }

        [XmlElement("DepartmentName")]
        [ColumnName("DEPT_NAME")]
        public string DepartmentName { get; set; }

        [XmlElement("DeductionCode")]
        [ColumnName("DEDUCT_CODE")]
        public string DeductionCode { get; set; }

        // Deduction Details (13-19)
        [XmlElement("DeductionDescription")]
        [ColumnName("DEDUCT_DESC")]
        public string DeductionDescription { get; set; }

        [XmlElement("BalanceAmount")]
        [ColumnName("OUTSTANDING_BAL")]
        public decimal BalanceAmount { get; set; }

        [XmlElement("DeductionAmount")]
        [ColumnName("MONTHLY_DEDUCTION")]
        public decimal DeductionAmount { get; set; }

        [XmlElement("HasStopPayStatus")]
        [ColumnName("STOP_PAY_FLAG")]
        public bool? HasStopPayStatus { get; set; }

        [XmlElement("StopPayReason")]
        [ColumnName("STOP_PAY_REASON")]
        public string StopPayReason { get; set; }

        [XmlElement("CheckDate")]
        [ColumnName("PAYROLL_DATE")]
        public DateTime CheckDate { get; set; }
    }
}
