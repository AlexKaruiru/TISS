using BRGateway24.Models;

namespace BRGateway24.Repository.AuditRepo
{
    public interface IAuditRepo
    {
        public Task<AuditResponseModel> GetEventsAsync(AuditRequestModel request);

    }
}