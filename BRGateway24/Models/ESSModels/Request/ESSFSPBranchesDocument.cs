using System.Xml.Serialization;
using static BRGateway24.Helpers.ESS.Dto.LoanDTO;

namespace BRGateway24.Models.ESSModels.Request
{
    [Serializable]
    [XmlRoot("Document")]
    public class ESSFSPBranchesDocument : ESSBaseDocument<ESSFSPBranchesHeader, ESSFSPBranchesBody>
    {
        // Add any FSP branch-specific document extensions here
        // Example: Custom methods for branch data processing
    }

    public class ESSFSPBranchesHeader : ESSBaseHeader
    {
        [XmlElement("FSPRegion")]
        public string Region { get; set; }

        [XmlElement("BranchCount")]
        public int BranchCount { get; set; }
    }

    public class ESSFSPBranchesBody
    {
        [XmlElement("DistrictCode")]
        [ColumnName("DISTRICT_CODE")]
        public string DistrictCode { get; set; }

        [XmlElement("BranchCode")]
        [ColumnName("BRANCH_CODE")]
        public string BranchCode { get; set; }

        [XmlElement("BranchName")]
        [ColumnName("BRANCH_NAME")]
        public string BranchName { get; set; }

        [XmlElement("BranchAddress")]
        [ColumnName("BRANCH_ADDRESS")]
        public string BranchAddress { get; set; }

        [XmlElement("ContactNumber")]
        [ColumnName("CONTACT_NUMBER")]
        public string ContactNumber { get; set; }

        [XmlElement("IsActive")]
        [ColumnName("IS_ACTIVE")]
        public bool IsActive { get; set; } = true;

        [XmlElement("LastUpdated")]
        [ColumnName("LAST_UPDATED")]
        public DateTime LastUpdated { get; set; } = DateTime.UtcNow;
    }
}
