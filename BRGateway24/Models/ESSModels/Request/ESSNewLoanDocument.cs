using System;
using System.Xml.Serialization;

namespace BRGateway24.Models.ESSModels.Request
{

    [Serializable]
    [XmlRoot("Document")]
    public class ESSNewLoanChargesDocument : ESSBaseDocument<ESSNewLoanHeader, ESSNewLoanChargesRequest>
    {
        
    }

    [Serializable]
    public class ESSNewLoanHeader : ESSBaseHeader
    {
        
    }

    [Serializable]
    public class ESSNewLoanChargesRequest
    {
        [XmlElement("CheckNumber")]
        public long CheckNumber { get; set; }

        [XmlElement("DesignationCode")]
        public string DesignationCode { get; set; }

        [XmlElement("DesignationName")]
        public string DesignationName { get; set; }

        [XmlElement("BasicSalary")]
        public decimal BasicSalary { get; set; }

        [XmlElement("NetSalary")]
        public decimal NetSalary { get; set; }

        [XmlElement("OneThirdAmount")]
        public decimal OneThirdAmount { get; set; }

        [XmlElement("RequestedAmount")]
        public decimal? RequestedAmount { get; set; }

        [XmlElement("DeductibleAmount")]
        public decimal DeductibleAmount { get; set; }

        [XmlElement("DesiredDeductibleAmount")]
        public decimal? DesiredDeductibleAmount { get; set; }

        [XmlElement("RetirementDate")]
        public int RetirementDate { get; set; }

        [XmlElement("TermsOfEmployment")]
        public string TermsOfEmployment { get; set; }

        [XmlElement("Tenure")]
        public int? Tenure { get; set; }

        [XmlElement("ProductCode")]
        public string ProductCode { get; set; }

        [XmlElement("VoteCode")]
        public string VoteCode { get; set; }

        [XmlElement("TotalEmployeeDeduction")]
        public decimal TotalEmployeeDeduction { get; set; }

        [XmlElement("JobClassCode")]
        public string JobClassCode { get; set; }
    }

}
