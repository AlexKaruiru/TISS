using BRGateway24.Models;
using Microsoft.Extensions.Primitives;

namespace BRGateway24.Repository.TISS
{
    public interface ITISSParticipantRepo
    {
        Task<MainResponse> GetBusinessDateAsync(TissApiHeaders headers);
        Task<MainResponse> GetCurrentTimetableEventAsync(TissApiHeaders headers);
        Task<MainResponse> GetPendingTransactionsAsync(string participantId, string currency, TissApiHeaders headers);
        Task<MainResponse> GetAccountActivitiesAsync(string participantId, string accountId,
            DateTime? fromDate, DateTime? toDate, string currency, TissApiHeaders headers);
        Task<MainResponse> SendMessageAsync(
            string messageType, string payloadXml, string reference, TissApiHeaders headers);
        Task<bool> ValidateTokenAsync(string token);
        Task<TissApiHeaders> GetTissHeadersByToken(string token);
    }
}