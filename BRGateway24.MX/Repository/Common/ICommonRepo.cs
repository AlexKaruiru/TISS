using BRGateway24.Models;

namespace BRGateway24.Repository.Common
{
    public interface ICommonRepo
    {
        
        public Task<AuditResp> APIAuditLogAsync(APIAuditLogRequest request);      
        public MainResponse InvokeResponse();
        public MainResponse InvokeResponse(string msg);
        public MainResponse InvokeResponse(string msg,string code);
        public MainResponse WaitingMessageResponse();


        public Task<MainResponse> GetCodesAsync(CodeDetails codeDetails);
        public Task LogRequestAsync(object request);
        

    }
}
