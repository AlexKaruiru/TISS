
using System.Xml.Serialization;
using static BRGateway24.Helpers.ESS.Dto.LoanDTO;

namespace BRGateway24.Models.ESSModels.Request
{
    // Loan Restructuring Request
    public class ESSLoanRestructuringDocument
    {
        // Employee Identification
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

        // Employment Details
        [XmlElement("MaritalStatus")]
        [ColumnName("MARITAL_STATUS")]
        public string MaritalStatus { get; set; }

        [XmlElement("NearestBranchName")]
        [ColumnName("NEAREST_BRANCH")]
        public string NearestBranchName { get; set; }

        [XmlElement("NearestBranchCode")]
        [ColumnName("BRANCH_CODE")]
        public string NearestBranchCode { get; set; }

        [XmlElement("VoteCode")]
        [ColumnName("VOTE_CODE")]
        public string VoteCode { get; set; }

        // Employer Information
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

        // Salary Information
        [XmlElement("BasicSalary")]
        [ColumnName("BASIC_SALARY")]
        public decimal BasicSalary { get; set; }

        [XmlElement("NetSalary")]
        [ColumnName("NET_SALARY")]
        public decimal NetSalary { get; set; }

        [XmlElement("OneThirdAmount")]
        [ColumnName("ONE_THIRD_AMOUNT")]
        public decimal OneThirdAmount { get; set; }

        [XmlElement("TotalEmployeeDeduction")]
        [ColumnName("TOTAL_DEDUCTIONS")]
        public decimal TotalEmployeeDeduction { get; set; }

        [XmlElement("RetirementDate")]
        [ColumnName("RETIREMENT_YEARS")]
        public int RetirementDate { get; set; }

        [XmlElement("TermsOfEmployment")]
        [ColumnName("EMPLOYMENT_TYPE")]
        public string TermsOfEmployment { get; set; }

        // Loan Restructuring Details
        [XmlElement("DesiredDeductibleAmount")]
        [ColumnName("DESIRED_DEDUCTION")]
        public decimal? DesiredDeductibleAmount { get; set; }

        [XmlElement("Tenure")]
        [ColumnName("REPAYMENT_PERIOD")]
        public int Tenure { get; set; }

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

        // Contact Information
        [XmlElement("Insurance")]
        [ColumnName("INSURANCE_RATE")]
        public decimal Insurance { get; set; }

        [XmlElement("PhysicalAddress")]
        [ColumnName("PHYSICAL_ADDRESS")]
        public string PhysicalAddress { get; set; }

        [XmlElement("EmailAddress")]
        [ColumnName("EMAIL")]
        public string EmailAddress { get; set; }

        [XmlElement("MobileNumber")]
        [ColumnName("MOBILE_NO")]
        public string MobileNumber { get; set; }

        // Application Details
        [XmlElement("ApplicationNumber")]
        [ColumnName("APP_NO")]
        public string ApplicationNumber { get; set; }

        [XmlElement("ContractStartDate")]
        [ColumnName("CONTRACT_START")]
        public string ContractStartDate { get; set; }

        [XmlElement("ContractEndDate")]
        [ColumnName("CONTRACT_END")]
        public string ContractEndDate { get; set; }

        [XmlElement("LoanNumber")]
        [ColumnName("LOAN_NO")]
        public string LoanNumber { get; set; }

        // Additional Information
        [XmlElement("Funding")]
        [ColumnName("SALARY_SOURCE")]
        public string Funding { get; set; }

        [XmlElement("FSPReferenceNumber")]
        [ColumnName("FSP_REF_NO")]
        public string FSPReferenceNumber { get; set; }

        [XmlElement("LoanPurpose")]
        [ColumnName("RESTRUCTURE_REASON")]
        public string LoanPurpose { get; set; }
    }

    // Loan Restructuring Balance
    public class ESSLoanRestructureBalanceBody
    {
        // Employee Identification
        [XmlElement("CheckNumber")]
        [ColumnName("EMP_ID")]
        public long CheckNumber { get; set; }

        // Loan Information
        [XmlElement("LoanNumber")]
        [ColumnName("LOAN_NO")]
        public string LoanNumber { get; set; }

        // Employee Personal Details
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
        [ColumnName("DEDUCTION_AMT")]
        public decimal DeductionAmount { get; set; }

        [XmlElement("DeductionCode")]
        [ColumnName("DEDUCTION_CODE")]
        public string DeductionCode { get; set; }

        [XmlElement("DeductionName")]
        [ColumnName("DEDUCTION_NAME")]
        public string DeductionName { get; set; }

        [XmlElement("DeductionBalance")]
        [ColumnName("DEDUCTION_BAL")]
        public decimal DeductionBalance { get; set; }

        // Financial Service Provider
        [XmlElement("FSPCode")]
        [ColumnName("FSP_CODE")]
        public string FSPCode { get; set; }

        // Payment Information
        [XmlElement("PaymentOption")]
        [ColumnName("PAY_OPTION")]
        public string PaymentOption { get; set; }
    }
}
