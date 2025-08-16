using BRGateway24.DataAccess;
using BRGateway24.Helpers;
using BRGateway24.Models;
using Microsoft.Data.SqlClient;
using Newtonsoft.Json;
using SwiftApi.Repository.Security;
using System.Data;
using System.Xml;

namespace BRGateway24.Repository.TISS
{
    public class TISSParticipantRepo : ITISSParticipantRepo
    {
        private readonly AppSettings _appSettings;
        private readonly ISystemSecurity _systemSecurity;
        private readonly ITissClientService _tissClientService;
        private string _connString;

        public TISSParticipantRepo(
            AppSettings appSettings,
            ISystemSecurity systemSecurity,
            ITissClientService tissClientService)
        {
            _appSettings = appSettings;
            _systemSecurity = systemSecurity;
            _tissClientService = tissClientService;
        }

        private SqlConnection GetConnection()
        {
            _connString = _systemSecurity.GetConnectionString(
                _appSettings.DBType, _appSettings.DBServerName, _appSettings.DatabaseName,
                _appSettings.BRUserName, _appSettings.BRUserPassword, "BRGateway24API");
            return new SqlConnection(_connString);
        }

        public async Task<bool> ValidateTokenAsync(string token, string participantBic)
        {
            using var connection = GetConnection();
            using var command = new SqlCommand("sp_TISS_ValidateToken", connection)
            {
                CommandType = CommandType.StoredProcedure
            };

            command.Parameters.AddWithValue("@Token", token);
            command.Parameters.AddWithValue("@ParticipantBIC", participantBic);

            await connection.OpenAsync();
            using var reader = await command.ExecuteReaderAsync();

            if (await reader.ReadAsync())
            {
                return reader.GetBoolean(0); // IsValid
            }

            return false;
        }

        public async Task<MainResponse> GetBusinessDateAsync(TissApiHeaders headers)
        {
            var response = new MainResponse();
            try
            {
                var apiResponse = await _tissClientService.SendRequestAsync(
                    "businessdate",
                    HttpMethod.Get,
                    headers);

                var content = await apiResponse.Content.ReadAsStringAsync();

                if (apiResponse.IsSuccessStatusCode)
                {
                    response.resp = new Response
                    {
                        Status = "000",
                        Message = "Success",
                        OutputJSON = content
                    };
                }
                else
                {
                    response.resp = new Response
                    {
                        Status = apiResponse.StatusCode.ToString(),
                        Message = $"Error from TISS: {content}"
                    };
                }
            }
            catch (Exception ex)
            {
                response.resp = new Response
                {
                    Status = "500",
                    Message = $"Error retrieving business date: {ex.Message}"
                };
            }
            return response;
        }

        public async Task<MainResponse> GetCurrentTimetableEventAsync(TissApiHeaders headers)
        {
            var response = new MainResponse();
            try
            {
                var apiResponse = await _tissClientService.SendRequestAsync(
                    "timetable/current",
                    HttpMethod.Get,
                    headers);

                var content = await apiResponse.Content.ReadAsStringAsync();

                if (apiResponse.IsSuccessStatusCode)
                {
                    response.resp = new Response
                    {
                        Status = "000",
                        Message = "Success",
                        OutputJSON = content
                    };
                }
                else
                {
                    response.resp = new Response
                    {
                        Status = apiResponse.StatusCode.ToString(),
                        Message = $"Error from TISS: {content}"
                    };
                }
            }
            catch (Exception ex)
            {
                response.resp = new Response
                {
                    Status = "500",
                    Message = $"Error retrieving timetable event: {ex.Message}"
                };
            }
            return response;
        }

        public async Task<MainResponse> GetPendingTransactionsAsync(string participantId, string currency, TissApiHeaders headers)
        {
            var response = new MainResponse();
            try
            {
                var endpoint = $"transactions/pending?participantId={participantId}&currency={currency}";
                var apiResponse = await _tissClientService.SendRequestAsync(
                    endpoint,
                    HttpMethod.Get,
                    headers);

                var content = await apiResponse.Content.ReadAsStringAsync();

                if (apiResponse.IsSuccessStatusCode)
                {
                    response.resp = new Response
                    {
                        Status = "000",
                        Message = "Success",
                        OutputJSON = content
                    };
                }
                else
                {
                    response.resp = new Response
                    {
                        Status = apiResponse.StatusCode.ToString(),
                        Message = $"Error from TISS: {content}"
                    };
                }
            }
            catch (Exception ex)
            {
                response.resp = new Response
                {
                    Status = "500",
                    Message = $"Error retrieving pending transactions: {ex.Message}"
                };
            }
            return response;
        }

        public async Task<MainResponse> GetAccountActivitiesAsync(
            string participantId,
            string accountId,
            DateTime? fromDate,
            DateTime? toDate,
            string currency,
            TissApiHeaders headers)
        {
            var response = new MainResponse();
            try
            {
                var endpoint = $"accounts/activity?participantId={participantId}&currency={currency}";

                if (!string.IsNullOrEmpty(accountId))
                    endpoint += $"&accountId={accountId}";
                if (fromDate.HasValue)
                    endpoint += $"&fromDate={fromDate.Value.ToString("yyyy-MM-dd")}";
                if (toDate.HasValue)
                    endpoint += $"&toDate={toDate.Value.ToString("yyyy-MM-dd")}";

                var apiResponse = await _tissClientService.SendRequestAsync(
                    endpoint,
                    HttpMethod.Get,
                    headers);

                var content = await apiResponse.Content.ReadAsStringAsync();

                if (apiResponse.IsSuccessStatusCode)
                {
                    response.resp = new Response
                    {
                        Status = "000",
                        Message = "Success",
                        OutputJSON = content
                    };
                }
                else
                {
                    response.resp = new Response
                    {
                        Status = apiResponse.StatusCode.ToString(),
                        Message = $"Error from TISS: {content}"
                    };
                }
            }
            catch (Exception ex)
            {
                response.resp = new Response
                {
                    Status = "500",
                    Message = $"Error retrieving account activities: {ex.Message}"
                };
            }
            return response;
        }

        public async Task<MainResponse> SendMessageAsync(
            string messageId,
            string participantId,
            string messageType,
            string payloadXml,
            string reference,
            TissApiHeaders headers)
        {
            var response = new MainResponse();
            try
            {
                // Validate XML
                try
                {
                    var xmlDoc = new XmlDocument();
                    xmlDoc.LoadXml(payloadXml);
                }
                catch (XmlException xmlEx)
                {
                    response.resp = new Response
                    {
                        Status = "400",
                        Message = $"Invalid XML payload: {xmlEx.Message}"
                    };
                    return response;
                }

                // Add content type to headers for POST request
                headers.ContentType = "application/xml";

                var apiResponse = await _tissClientService.SendRequestAsync(
                    "messages",
                    HttpMethod.Post,
                    headers,
                    payloadXml);

                var content = await apiResponse.Content.ReadAsStringAsync();

                if (apiResponse.IsSuccessStatusCode)
                {
                    // Store the message in local DB for tracking
                    await StoreMessageLocally(messageId, participantId, messageType, payloadXml, reference);

                    response.resp = new Response
                    {
                        Status = "000",
                        Message = "Success",
                        OutputJSON = content
                    };
                }
                else
                {
                    response.resp = new Response
                    {
                        Status = apiResponse.StatusCode.ToString(),
                        Message = $"Error from TISS: {content}"
                    };
                }
            }
            catch (Exception ex)
            {
                response.resp = new Response
                {
                    Status = "500",
                    Message = $"Error sending message: {ex.Message}"
                };
            }
            return response;
        }

        private async Task StoreMessageLocally(
            string messageId,
            string participantId,
            string messageType,
            string payloadXml,
            string reference)
        {
            using var connection = GetConnection();
            using var command = new SqlCommand("sp_TISS_StoreMessage", connection)
            {
                CommandType = CommandType.StoredProcedure
            };

            command.Parameters.AddWithValue("@MessageID", messageId);
            command.Parameters.AddWithValue("@ParticipantID", participantId);
            command.Parameters.AddWithValue("@Direction", "OUT");
            command.Parameters.AddWithValue("@MessageType", messageType);
            if (!string.IsNullOrEmpty(reference))
                command.Parameters.AddWithValue("@Reference", reference);
            command.Parameters.AddWithValue("@Status", "SENT");
            command.Parameters.AddWithValue("@PayloadXML", payloadXml);

            try
            {
                var xmlDoc = new XmlDocument();
                xmlDoc.LoadXml(payloadXml);

                var amountNode = xmlDoc.SelectSingleNode("//IntrBkSttlmAmt");
                if (amountNode != null)
                {
                    if (decimal.TryParse(amountNode.InnerText, out decimal amount))
                    {
                        command.Parameters.AddWithValue("@Amount", amount);
                    }

                    var currency = amountNode.Attributes?["Ccy"]?.Value;
                    if (!string.IsNullOrEmpty(currency))
                    {
                        command.Parameters.AddWithValue("@Currency", currency);
                    }
                }
            }
            catch { /* Ignore extraction errors */ }

            await connection.OpenAsync();
            await command.ExecuteNonQueryAsync();
        }
    }
}