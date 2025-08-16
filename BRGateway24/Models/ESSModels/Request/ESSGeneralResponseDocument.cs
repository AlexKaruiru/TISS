using System.Xml.Serialization;
using static BRGateway24.Helpers.ESS.Dto.LoanDTO;

namespace BRGateway24.Models.ESSModels.Request
{
    [Serializable]
    [XmlRoot("Document")]
    public class ESSGeneralResponseDocument : ESSBaseDocument<ESSGeneralResponseHeader, ESSGeneralResponseBody>
    {
        public static ESSGeneralResponseDocument CreateSuccess(string message) =>
            Create(200, message);

        public static ESSGeneralResponseDocument CreateError(int errorCode, string errorMessage) =>
            Create(errorCode, errorMessage);

        private static ESSGeneralResponseDocument Create(int code, string message)
        {
            return new ESSGeneralResponseDocument
            {
                Data = new ESSBaseData<ESSGeneralResponseHeader, ESSGeneralResponseBody>
                {
                    Header = new ESSGeneralResponseHeader(),
                    MessageDetails = new ESSGeneralResponseBody
                    {
                        ResponseCode = code,
                        Description = message
                    }
                },
                Signature = GenerateDefaultSignature()
            };
        }

        private static string GenerateDefaultSignature()
        {
            return Convert.ToBase64String(Guid.NewGuid().ToByteArray())[..16];
        }
    }

    public class ESSGeneralResponseHeader : ESSBaseHeader
    {
        [XmlElement("ResponseTimestamp")]
        public DateTime ResponseTimestamp { get; set; } = DateTime.UtcNow;
    }

    public class ESSGeneralResponseBody
    {
        [XmlElement("ResponseCode")]
        [ColumnName("RESPONSE_CODE")]
        public int ResponseCode { get; set; }

        [XmlElement("Description")]
        [ColumnName("RESPONSE_MESSAGE")]
        public string Description { get; set; }

        // Optional extended error details
        [XmlElement("Details")]
        [ColumnName("ERROR_DETAILS")]
        public string Details { get; set; }

        [XmlElement("ReferenceId")]
        [ColumnName("REFERENCE_ID")]
        public string ReferenceId { get; set; }
    }
}
