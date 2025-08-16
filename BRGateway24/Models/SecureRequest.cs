using Newtonsoft.Json.Linq;
using System.Text.Json;

namespace BRNetTopazBio.Contracts
{
   public class SecureRequest
    {
        public required string route { get; set; }
        public required JObject data { get; set; }
    }

    public class EncryptedPayload
    {
        public required string EncryptedData { get; set; }
    }

    public class BodyModel
    {
        public required string memberID { get; set; }
        public required string sBioImage { get; set; }
    }

}
