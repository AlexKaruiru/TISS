using BRGateway24.Models;
using System.Xml.Linq;
using System.Xml.Serialization;

namespace BRGateway24.Repository.TISS.Mock
{
    public interface IMockTissService
    {
        Task<string> GetBusinessDateAsync(string currency);
        Task<string> GetCurrentTimetableEventAsync(string currency);
        Task<string> PostMessageAsync(TissApiHeaders headers, string xmlContent);
        Task<string> GetPendingTransactionsAsync(string sender, string currency, string authorization);
        Task<string> GetAccountsActivityAsync(string sender, string currency, string authorization);
    }

    public class MockTissService : IMockTissService
    {
        private readonly ILogger<MockTissService> _logger;

        public MockTissService(ILogger<MockTissService> logger)
        {
            _logger = logger;
        }

        public async Task<string> GetBusinessDateAsync(string currency)
        {
            _logger.LogInformation("Mock TISS: Returning business date for currency: {Currency}", currency);

            // Simulate async operation
            await Task.Delay(100);

            var currentDate = DateTime.Now.ToString("yyyyMMdd");
            return $"{{\"{currency}\": \"{currentDate}\"}}";
        }

        public async Task<string> GetCurrentTimetableEventAsync(string currency)
        {
            _logger.LogInformation("Mock TISS: Returning current timetable event for currency: {Currency}", currency);

            await Task.Delay(100);

            // Sample timetable events
            var events = new Dictionary<string, string>
            {
                { "SOD", "00:10" },
                { "EOD", "23:50" },
                { "BUSINESS_DAY", "08:00" }
            };

            var randomEvent = events.OrderBy(x => Guid.NewGuid()).First();
            return $"{{\"{currency}\": \"{randomEvent.Key}:{randomEvent.Value}\"}}";
        }

        public async Task<string> PostMessageAsync(TissApiHeaders headers, string xmlContent)
        {
            _logger.LogInformation("Mock TISS: Processing message with MsgId: {MsgId}", headers.MsgId);

            await Task.Delay(200); // Simulate processing time

            try
            {
                // Parse the incoming XML to extract information
                var xmlDoc = XDocument.Parse(xmlContent);

                // Extract message type from the XML
                var messageType = xmlDoc.Descendants()
                    .FirstOrDefault(x => x.Name.LocalName == "MessageIdentifier")?.Value ?? "unknown";

                // Extract sender reference
                var senderReference = xmlDoc.Descendants()
                    .FirstOrDefault(x => x.Name.LocalName == "SenderReference")?.Value ?? "unknown";

                // Generate a mock accepted response
                var response = new TissMessageResponse
                {
                    Header = new TissResponseHeader
                    {
                        Sender = headers.Sender,
                        Receiver = "TISS"
                    },
                    ResponseDetails = new TissResponseDetails
                    {
                        OrgMsgId = headers.MsgId,
                        RespStatus = "ACCEPTED",
                        RespReason = "Message successfully processed by mock service"
                    }
                };

                // Serialize to XML
                var serializer = new XmlSerializer(typeof(TissMessageResponse));
                using var writer = new StringWriter();
                serializer.Serialize(writer, response);
                return writer.ToString();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Mock TISS: Error processing message");

                // Return a rejection response
                var rejectionResponse = new TissMessageResponse
                {
                    Header = new TissResponseHeader
                    {
                        Sender = headers.Sender,
                        Receiver = "TISS"
                    },
                    ResponseDetails = new TissResponseDetails
                    {
                        OrgMsgId = headers.MsgId,
                        RespStatus = "REJECTED",
                        RespReason = $"Mock service error: {ex.Message}"
                    }
                };

                var serializer = new XmlSerializer(typeof(TissMessageResponse));
                using var writer = new StringWriter();
                serializer.Serialize(writer, rejectionResponse);
                return writer.ToString();
            }
        }

        public async Task<string> GetPendingTransactionsAsync(string sender, string currency, string authorization)
        {
            _logger.LogInformation("Mock TISS: Returning pending transactions for sender: {Sender}", sender);

            await Task.Delay(150);

            // Generate mock pending transactions
            var pendingTransactions = new
            {
                transactions = new[]
                {
                    new
                    {
                        type = "SCCT",
                        reference = $"REF{DateTime.Now:yyyyMMddHHmmss}1",
                        debtor = sender,
                        creditor = "WXYZTZTX",
                        currency = currency,
                        amount = "100000000.00"
                    },
                    new
                    {
                        type = "SCCT",
                        reference = $"REF{DateTime.Now:yyyyMMddHHmmss}2",
                        debtor = "WXYZTZTX",
                        creditor = sender,
                        currency = currency,
                        amount = "50000000.00"
                    }
                }
            };

            return System.Text.Json.JsonSerializer.Serialize(pendingTransactions);
        }

        public async Task<string> GetAccountsActivityAsync(string sender, string currency, string authorization)
        {
            _logger.LogInformation("Mock TISS: Returning accounts activity for sender: {Sender}", sender);

            await Task.Delay(150);

            // Generate mock account activity
            var accountsActivity = new
            {
                accounts = new[]
                {
                    new
                    {
                        accountType = "SETTLEMENT_ACCOUNT",
                        accountNumber = "9922182101",
                        openingBalance = "19212562809.36",
                        completeTransactionsBalance = "0.00",
                        availableBalance = "19358857060.53",
                        currency = currency
                    },
                    new
                    {
                        accountType = "RESERVE_REQUIREMENT_ACCOUNT",
                        accountNumber = "RESERVE_REQUIREMENT_ACCOUNT",
                        openingBalance = "731471255.87",
                        completeTransactionsBalance = "0.00",
                        availableBalance = "731471255.87",
                        currency = currency
                    },
                    new
                    {
                        accountType = "ILF_ACCOUNT",
                        accountNumber = "0000000543",
                        openingBalance = "0.00",
                        completeTransactionsBalance = "0.00",
                        availableBalance = "0.00",
                        currency = currency
                    }
                }
            };

            return System.Text.Json.JsonSerializer.Serialize(accountsActivity);
        }
    }
}