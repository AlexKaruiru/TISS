using System.Xml.Serialization;

namespace BRGateway24.Models.ESSModels.Request
{
    [Serializable]
    [XmlRoot("Document")]
    public class ESSBaseDocument<THeader, TBody>
    where THeader : ESSBaseHeader
    where TBody : class
    {
        [XmlElement("Data")]
        public ESSBaseData<THeader, TBody> Data { get; set; }

        [XmlElement("Signature")]
        public string Signature { get; set; }

        public static ESSBaseDocument<THeader, TBody> Create(THeader header, TBody body, string signature = null)
        {
            return new ESSBaseDocument<THeader, TBody>
            {
                Data = ESSBaseData<THeader, TBody>.Create(header, body), 
                Signature = signature ?? GenerateDefaultSignature()
            };
        }

        private static string GenerateDefaultSignature()
        {
            return Convert.ToBase64String(Guid.NewGuid().ToByteArray())[..16];
        }
    }

    [Serializable]
    public class ESSBaseData<THeader, TBody>
    where THeader : ESSBaseHeader 
    where TBody : class
{
    [XmlElement("Header")]
    public THeader Header { get; set; }

    [XmlElement("MessageDetails")]
    public TBody MessageDetails { get; set; }

    public static ESSBaseData<THeader, TBody> Create(THeader header, TBody body)
    {
        return new ESSBaseData<THeader, TBody>
        {
            Header = header,
            MessageDetails = body
        };
    }
}

    [Serializable]
    public class ESSBaseHeader
    {
        [XmlElement("Sender")]
        public string Sender { get; set; } = "ESS_UTUMISHI";

        [XmlElement("Receiver")]
        public string Receiver { get; set; } = "FSP_SYSTEM";

        [XmlElement("FSPCode")]
        public string FSPCode { get; set; }

        [XmlElement("MsgId")]
        public string MessageId { get; set; } = Guid.NewGuid().ToString();

        [XmlElement("MessageType")]
        public string MessageType { get; set; }

        [XmlElement("Timestamp")]
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;

        [XmlElement("Version")]
        public string Version { get; set; } = "1.0";
    }
}
