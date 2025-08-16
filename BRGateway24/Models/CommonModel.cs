namespace BRGateway24.Models
{
    public class CommonModel
    {
    }

    public class HeaderRequest
    {
        public string ConsumerKey { get; set; }
        public string ConsumerSecret { get; set; }

    }
    public class LoginReq
    {
        public MainResponse response { get; set; }
        public string AccessToken { get; set; }
    }
    public class MainResponse
    {
        public string OurBranchID { get; set; }
        public Response resp { get; set; }
    }

    public class Response
    {
        public string Status { get; set; }
        public string OutputJSON { get; set; }
        public string Message { get; set; }

    }

    public class InvokeRequest
    {
        public string resp { get; set; }
        public string UniqueKey { get; set; }

    }

    public class InvokeMainResponse
    {
        public Response response { get; set; }
        public InvokeResponse invokeResponse { get; set; }
    }

    public class InvokeResponse
    {
        public string resp { get; set; }

    }

    public class AuditResp
    {
        string resp;
    }

    public class APIAuditLogRequest
    {
        public string MethodName { get; set; }
        public string Action { get; set; }
        public string AuditID { get; set; }
        public string DetailRecords { get; set; }
        public string Request { get; set; }
        public string Response { get; set; }

    }


    public class CodeDetails
    {
        public string ID { get; set; }        
    }
}
