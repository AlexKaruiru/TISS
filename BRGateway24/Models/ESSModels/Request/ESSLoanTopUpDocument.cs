
using System.Xml.Serialization;
using static BRGateway24.Helpers.ESS.Dto.LoanDTO;

namespace BRGateway24.Models.ESSModels.Request
{
    // Loan Top Up Request
    public class ESSLoanTopUpDocument
    {
        // Employee Identification (1-6)
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

        [XmlElement("Sex")]
        [ColumnName("GENDER")]
        public string Sex { get; set; }

        [XmlElement("EmploymentDate")]
        [ColumnName("EMP_DATE")]
        public string EmploymentDate { get; set; }

        // Personal Details (7-11)
        [XmlElement("MaritalStatus")]
        [ColumnName("MARITAL_STATUS")]
        public string MaritalStatus { get; set; }

        [XmlElement("ConfirmationDate")]
        [ColumnName("CONFIRM_DATE")]
        public string ConfirmationDate { get; set; }

        [XmlElement("BankAccountNumber")]
        [ColumnName("BANK_ACCOUNT")]
        public string BankAccountNumber { get; set; }

        [XmlElement("NearestBranchName")]
        [ColumnName("NEAREST_BRANCH")]
        public string NearestBranchName { get; set; }

        [XmlElement("NearestBranchCode")]
        [ColumnName("BRANCH_CODE")]
        public string NearestBranchCode { get; set; }

        // Employer Information (12-16)
        [XmlElement("VoteCode")]
        [ColumnName("VOTE_CODE")]
        public string VoteCode { get; set; }

        [XmlElement("VoteName")]
        [ColumnName("EMPLOYER_NAME")]
        public string VoteName { get; set; }

        [XmlElement("NIN")]
        [ColumnName("NATIONAL_ID")]
        public string NationalIdentificationNumber { get; set; }

        [XmlElement("DesignationCode")]
        [ColumnName("DESIG_CODE")]
        public string DesignationCode { get; set; }

        [XmlElement("DesignationName")]
        [ColumnName("DESIG_NAME")]
        public string DesignationName { get; set; }

        // Financial Information (17-26)
        [XmlElement("BasicSalary")]
        [ColumnName("BASIC_SALARY")]
        public decimal BasicSalary { get; set; }

        [XmlElement("NetSalary")]
        [ColumnName("NET_SALARY")]
        public decimal NetSalary { get; set; }

        [XmlElement("OneThirdAmount")]
        [ColumnName("ONE_THIRD_AMT")]
        public decimal OneThirdAmount { get; set; }

        [XmlElement("TotalEmployeeDeduction")]
        [ColumnName("TOTAL_DEDUCTIONS")]
        public decimal TotalEmployeeDeduction { get; set; }

        [XmlElement("RetirementDate")]
        [ColumnName("RETIREMENT_YRS")]
        public int RetirementDate { get; set; }

        [XmlElement("TermsOfEmployment")]
        [ColumnName("EMPLOYMENT_TYPE")]
        public string TermsOfEmployment { get; set; }

        [XmlElement("RequestedAmount")]
        [ColumnName("REQUESTED_AMT")]
        public decimal? RequestedAmount { get; set; }

        [XmlElement("DesiredDeductibleAmount")]
        [ColumnName("DESIRED_DEDUCTION")]
        public decimal? DesiredDeductibleAmount { get; set; }

        [XmlElement("Tenure")]
        [ColumnName("REPAYMENT_PERIOD")]
        public int Tenure { get; set; }

        // FSP Details (27-34)
        [XmlElement("FSPCode")]
        [ColumnName("FSP_CODE")]
        public string FSPCode { get; set; }

        [XmlElement("ProductCode")]
        [ColumnName("PRODUCT_CODE")]
        public string ProductCode { get; set; }

        [XmlElement("InterestRate")]
        [ColumnName("INTEREST_RATE")]
        public decimal InterestRate { get; set; }

        [XmlElement("ProcessingFee")]
        [ColumnName("PROCESSING_FEE")]
        public decimal ProcessingFee { get; set; }

        [XmlElement("Insurance")]
        [ColumnName("INSURANCE_AMT")]
        public decimal Insurance { get; set; }

        [XmlElement("PhysicalAddress")]
        [ColumnName("PHYSICAL_ADDR")]
        public string PhysicalAddress { get; set; }

        [XmlElement("TelephoneNumber")]
        [ColumnName("PHONE_NO")]
        public string TelephoneNumber { get; set; }

        [XmlElement("EmailAddress")]
        [ColumnName("EMAIL")]
        public string EmailAddress { get; set; }

        // Loan Details (35-42)
        [XmlElement("MobileNumber")]
        [ColumnName("MOBILE_NO")]
        public string MobileNumber { get; set; }

        [XmlElement("ApplicationNumber")]
        [ColumnName("APP_NO")]
        public string ApplicationNumber { get; set; }

        [XmlElement("LoanPurpose")]
        [ColumnName("LOAN_PURPOSE")]
        public string LoanPurpose { get; set; }

        [XmlElement("ContractStartDate")]
        [ColumnName("CONTRACT_START")]
        public string ContractStartDate { get; set; }

        [XmlElement("ContractEndDate")]
        [ColumnName("CONTRACT_END")]
        public string ContractEndDate { get; set; }

        [XmlElement("LoanNumber")]
        [ColumnName("LOAN_NO")]
        public string LoanNumber { get; set; }

        [XmlElement("SettlementAmount")]
        [ColumnName("SETTLEMENT_AMT")]
        public decimal SettlementAmount { get; set; }

        [XmlElement("SwiftCode")]
        [ColumnName("SWIFT_CODE")]
        public string SwiftCode { get; set; }

        [XmlElement("Funding")]
        [ColumnName("SALARY_SOURCE")]
        public string Funding { get; set; }
    }
}
