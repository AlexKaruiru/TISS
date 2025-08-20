using System.Xml.Serialization;

namespace BRGateway24.Repository.TISS.Mock
{
    [XmlRoot("Document")]
    public class TissMessageResponse
    {
        [XmlElement("header")]
        public TissResponseHeader Header { get; set; }

        [XmlElement("responseDetails")]
        public TissResponseDetails ResponseDetails { get; set; }
    }

    public class TissResponseHeader
    {
        [XmlElement("sender")]
        public string Sender { get; set; }

        [XmlElement("receiver")]
        public string Receiver { get; set; }
    }

    public class TissResponseDetails
    {
        [XmlElement("OrgMsgId")]
        public string OrgMsgId { get; set; }

        [XmlElement("RespStatus")]
        public string RespStatus { get; set; }

        [XmlElement("RespReason")]
        public string RespReason { get; set; }
    }

}