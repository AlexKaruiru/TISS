using BRGateway24.Models;
using System.Threading.Tasks;

namespace BRGateway24.Repository.TISS
{
   public interface ITISSParticipantRepo
    {
        Task<TissApiHeaders> GetTissApiHeaders(string configName = "Default");
        Task<MainResponse> GetBusinessDateAsync(TissApiHeaders headers);
        Task<MainResponse> GetCurrentTimetableEventAsync(TissApiHeaders headers);
        Task<MainResponse> GetPendingTransactionsAsync(TissApiHeaders headers);
        Task<MainResponse> GetAccountActivitiesAsync(
            string accountId,
            DateTime? fromDate,
            DateTime? toDate,
            TissApiHeaders headers);
        Task<MainResponse> SendMessageAsync(
            string messageType,
            string payloadXml,
            string reference,
            TissApiHeaders headers);
    }
}

