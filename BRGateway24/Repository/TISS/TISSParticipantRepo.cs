using BRGateway24.DataAccess;
using BRGateway24.Helpers;
using BRGateway24.Models;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using SwiftApi.Repository.Security;
using System;
using System.Data;
using System.Net.Http;
using System.Threading.Tasks;
using System.Xml;

namespace BRGateway24.Repository.TISS
{
    public class TISSParticipantRepo : ITISSParticipantRepo
    {
        private readonly AppSettings _appSettings;
        private readonly ISystemSecurity _systemSecurity;
        private readonly ITissClientService _tissClientService;
        private readonly ILogger<TISSParticipantRepo> _logger;
        private string _connString;

        public TISSParticipantRepo(
            AppSettings appSettings,
            ISystemSecurity systemSecurity,
            ITissClientService tissClientService,
            ILogger<TISSParticipantRepo> logger)
        {
            _appSettings = appSettings;
            _systemSecurity = systemSecurity;
            _tissClientService = tissClientService;
            _logger = logger;
        }

        private SqlConnection GetConnection()
        {
            _connString = _systemSecurity.GetConnectionString(
                _appSettings.DBType, _appSettings.DBServerName, _appSettings.DatabaseName,
                _appSettings.BRUserName, _appSettings.BRUserPassword, "BRGateway24API");
            return new SqlConnection(_connString);
        }

        public async Task<TissApiHeaders> GetTissApiHeaders(string configName = "Default")
        {
            try
            {
                using var connection = GetConnection();
                using var command = new SqlCommand("p_GetMXApiHeaders", connection)
                {
                    CommandType = CommandType.StoredProcedure
                };

                command.Parameters.AddWithValue("@ConfigName", configName);

                await connection.OpenAsync();
                using var reader = await command.ExecuteReaderAsync();

                if (await reader.ReadAsync())
                {
                    return new TissApiHeaders
                    {
                        Authorization = reader["Authorization"]?.ToString(),
                        Sender = reader["Sender"]?.ToString(),
                        Consumer = reader["Consumer"]?.ToString(),
                        ContentType = reader["ContentType"]?.ToString(),
                        PayloadType = reader["PayloadType"]?.ToString(),
                        Currency = reader["Currency"]?.ToString() ?? "TZS",
                        MsgId = $"MSG_{Guid.NewGuid():N}"
                    };
                }

                _logger.LogError("No TISS API headers configuration found for {ConfigName}", configName);
                return null;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving TISS API headers for {ConfigName}", configName);
                return null;
            }
        }

        private async Task<long> LogRequest(TissApiRequest request)
        {
            try
            {
                using var connection = GetConnection();
                using var command = new SqlCommand("p_MXLogApiRequest", connection)
                {
                    CommandType = CommandType.StoredProcedure
                };

                command.Parameters.AddWithValue("@MessageID", request.MessageID);
                command.Parameters.AddWithValue("@Endpoint", request.Endpoint);
                command.Parameters.AddWithValue("@Method", request.Method);
                command.Parameters.AddWithValue("@Headers", request.Headers);
                command.Parameters.AddWithValue("@RequestBody",
                    string.IsNullOrEmpty(request.RequestBody) ? (object)DBNull.Value : request.RequestBody);
                command.Parameters.AddWithValue("@ParticipantID",
                    string.IsNullOrEmpty(request.ParticipantID) ? (object)DBNull.Value : request.ParticipantID);

                var requestIdParam = new SqlParameter("@RequestID", SqlDbType.BigInt)
                {
                    Direction = ParameterDirection.Output
                };
                command.Parameters.Add(requestIdParam);

                await connection.OpenAsync();
                await command.ExecuteNonQueryAsync();

                return requestIdParam.Value != DBNull.Value ? (long)requestIdParam.Value : 0;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to log TISS API request");
                return 0;
            }
        }

        private async Task LogResponse(TissApiResponse response)
        {
            try
            {
                using var connection = GetConnection();
                using var command = new SqlCommand("p_MXLogApiResponse", connection)
                {
                    CommandType = CommandType.StoredProcedure
                };

                command.Parameters.AddWithValue("@RequestID", response.RequestID);
                command.Parameters.AddWithValue("@StatusCode", response.StatusCode);
                command.Parameters.AddWithValue("@ResponseBody",
                    string.IsNullOrEmpty(response.ResponseBody) ? (object)DBNull.Value : response.ResponseBody);
                command.Parameters.AddWithValue("@ErrorDetails",
                    string.IsNullOrEmpty(response.ErrorDetails) ? (object)DBNull.Value : response.ErrorDetails);

                await connection.OpenAsync();
                await command.ExecuteNonQueryAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to log TISS API response for RequestID: {RequestID}", response.RequestID);
            }
        }

        public async Task<MainResponse> GetBusinessDateAsync(TissApiHeaders headers)
        {
            var response = new MainResponse();
            long requestId = 0;

            try
            {
                requestId = await LogRequest(new TissApiRequest
                {
                    MessageID = headers.MsgId,
                    Endpoint = "interface/participants/businessDate",
                    Method = "GET",
                    Headers = headers.ToJson(),
                    ParticipantID = headers.Sender
                });

                var apiResponse = await _tissClientService.SendRequestAsync(
                    "interface/participants/businessDate", HttpMethod.Get, headers);

                var content = await apiResponse.Content.ReadAsStringAsync();

                await LogResponse(new TissApiResponse
                {
                    RequestID = requestId,
                    StatusCode = (int)apiResponse.StatusCode,
                    ResponseBody = content,
                    ErrorDetails = apiResponse.IsSuccessStatusCode ? null : content
                });

                // Return exact TISS response without modification
                response.resp = new Response
                {
                    Status = ((int)apiResponse.StatusCode).ToString(),
                    Message = content,
                    OutputJSON = apiResponse.IsSuccessStatusCode ? content : null
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in GetBusinessDateAsync");

                if (requestId > 0)
                {
                    await LogResponse(new TissApiResponse
                    {
                        RequestID = requestId,
                        StatusCode = 500,
                        ErrorDetails = ex.Message
                    });
                }

                response.resp = new Response
                {
                    Status = "500",
                    Message = ex.Message
                };
            }

            return response;
        }

        public async Task<MainResponse> GetCurrentTimetableEventAsync(TissApiHeaders headers)
        {
            var response = new MainResponse();
            long requestId = 0;

            try
            {
                requestId = await LogRequest(new TissApiRequest
                {
                    MessageID = headers.MsgId,
                    Endpoint = "interface/participants/currentTimetableEvent",
                    Method = "GET",
                    Headers = headers.ToJson(),
                    ParticipantID = headers.Sender
                });

                var apiResponse = await _tissClientService.SendRequestAsync(
                    "interface/participants/currentTimetableEvent", HttpMethod.Get, headers);

                var content = await apiResponse.Content.ReadAsStringAsync();

                await LogResponse(new TissApiResponse
                {
                    RequestID = requestId,
                    StatusCode = (int)apiResponse.StatusCode,
                    ResponseBody = content,
                    ErrorDetails = apiResponse.IsSuccessStatusCode ? null : content
                });

                response.resp = new Response
                {
                    Status = ((int)apiResponse.StatusCode).ToString(),
                    Message = content,
                    OutputJSON = apiResponse.IsSuccessStatusCode ? content : null
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in GetCurrentTimetableEventAsync");

                if (requestId > 0)
                {
                    await LogResponse(new TissApiResponse
                    {
                        RequestID = requestId,
                        StatusCode = 500,
                        ErrorDetails = ex.Message
                    });
                }

                response.resp = new Response
                {
                    Status = "500",
                    Message = ex.Message
                };
            }

            return response;
        }

        public async Task<MainResponse> GetPendingTransactionsAsync(string participantId, string currency, TissApiHeaders headers)
        {
            var response = new MainResponse();
            long requestId = 0;

            try
            {
                var endpoint = $"interface/participants/pendingTransactions?sender={participantId}&currency={currency}";

                requestId = await LogRequest(new TissApiRequest
                {
                    MessageID = headers.MsgId,
                    Endpoint = endpoint,
                    Method = "GET",
                    Headers = headers.ToJson(),
                    ParticipantID = headers.Sender
                });

                var apiResponse = await _tissClientService.SendRequestAsync(
                    endpoint, HttpMethod.Get, headers);

                var content = await apiResponse.Content.ReadAsStringAsync();

                await LogResponse(new TissApiResponse
                {
                    RequestID = requestId,
                    StatusCode = (int)apiResponse.StatusCode,
                    ResponseBody = content,
                    ErrorDetails = apiResponse.IsSuccessStatusCode ? null : content
                });

                response.resp = new Response
                {
                    Status = ((int)apiResponse.StatusCode).ToString(),
                    Message = content,
                    OutputJSON = apiResponse.IsSuccessStatusCode ? content : null
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in GetPendingTransactionsAsync");

                if (requestId > 0)
                {
                    await LogResponse(new TissApiResponse
                    {
                        RequestID = requestId,
                        StatusCode = 500,
                        ErrorDetails = ex.Message
                    });
                }

                response.resp = new Response
                {
                    Status = "500",
                    Message = ex.Message
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
            long requestId = 0;

            try
            {
                var endpoint = $"interface/participants/accountsActivity?sender={participantId}&currency={currency}";

                if (!string.IsNullOrEmpty(accountId))
                    endpoint += $"&accountId={Uri.EscapeDataString(accountId)}";
                if (fromDate.HasValue)
                    endpoint += $"&fromDate={fromDate.Value:yyyy-MM-dd}";
                if (toDate.HasValue)
                    endpoint += $"&toDate={toDate.Value:yyyy-MM-dd}";

                requestId = await LogRequest(new TissApiRequest
                {
                    MessageID = headers.MsgId,
                    Endpoint = endpoint,
                    Method = "GET",
                    Headers = headers.ToJson(),
                    ParticipantID = headers.Sender
                });

                var apiResponse = await _tissClientService.SendRequestAsync(
                    endpoint, HttpMethod.Get, headers);

                var content = await apiResponse.Content.ReadAsStringAsync();

                await LogResponse(new TissApiResponse
                {
                    RequestID = requestId,
                    StatusCode = (int)apiResponse.StatusCode,
                    ResponseBody = content,
                    ErrorDetails = apiResponse.IsSuccessStatusCode ? null : content
                });

                response.resp = new Response
                {
                    Status = ((int)apiResponse.StatusCode).ToString(),
                    Message = content,
                    OutputJSON = apiResponse.IsSuccessStatusCode ? content : null
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in GetAccountActivitiesAsync");

                if (requestId > 0)
                {
                    await LogResponse(new TissApiResponse
                    {
                        RequestID = requestId,
                        StatusCode = 500,
                        ErrorDetails = ex.Message
                    });
                }

                response.resp = new Response
                {
                    Status = "500",
                    Message = ex.Message
                };
            }

            return response;
        }

        public async Task<MainResponse> SendMessageAsync(
            string messageType,
            string payloadXml,
            string reference,
            TissApiHeaders headers)
        {
            var response = new MainResponse();
            long requestId = 0;

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

                // Set message-specific headers
                headers.ContentType = "application/xml";
                headers.MsgId = reference;

                requestId = await LogRequest(new TissApiRequest
                {
                    MessageID = headers.MsgId,
                    Endpoint = "interface/participants/message",
                    Method = "POST",
                    Headers = headers.ToJson(),
                    RequestBody = payloadXml,
                    ParticipantID = headers.Sender
                });

                var apiResponse = await _tissClientService.SendRequestAsync(
                    "interface/participants/message", HttpMethod.Post, headers, payloadXml);

                var content = await apiResponse.Content.ReadAsStringAsync();

                await LogResponse(new TissApiResponse
                {
                    RequestID = requestId,
                    StatusCode = (int)apiResponse.StatusCode,
                    ResponseBody = content,
                    ErrorDetails = apiResponse.IsSuccessStatusCode ? null : content
                });

                if (apiResponse.IsSuccessStatusCode)
                {
                    await StoreMessageLocally(messageType, payloadXml, reference);
                }

                response.resp = new Response
                {
                    Status = ((int)apiResponse.StatusCode).ToString(),
                    Message = content,
                    OutputJSON = apiResponse.IsSuccessStatusCode ? content : null
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in SendMessageAsync");

                if (requestId > 0)
                {
                    await LogResponse(new TissApiResponse
                    {
                        RequestID = requestId,
                        StatusCode = 500,
                        ErrorDetails = ex.Message
                    });
                }

                response.resp = new Response
                {
                    Status = "500",
                    Message = ex.Message
                };
            }

            return response;
        }

        private async Task StoreMessageLocally(string messageType, string payloadXml, string reference)
        {
            try
            {
                using var connection = GetConnection();
                using var command = new SqlCommand("sp_TISS_StoreMessage", connection)
                {
                    CommandType = CommandType.StoredProcedure
                };

                command.Parameters.AddWithValue("@MessageType", messageType);
                command.Parameters.AddWithValue("@Direction", "OUT");
                command.Parameters.AddWithValue("@Reference", reference);
                command.Parameters.AddWithValue("@Status", "SENT");
                command.Parameters.AddWithValue("@PayloadXML", payloadXml);

                // Try to extract amount and currency from XML
                try
                {
                    var xmlDoc = new XmlDocument();
                    xmlDoc.LoadXml(payloadXml);

                    var namespaceManager = new XmlNamespaceManager(xmlDoc.NameTable);
                    namespaceManager.AddNamespace("ns", "urn:iso:std:iso:20022:tech:xsd:pacs.008.001.08");

                    var amountNode = xmlDoc.SelectSingleNode("//ns:IntrBkSttlmAmt", namespaceManager) ??
                                    xmlDoc.SelectSingleNode("//IntrBkSttlmAmt");

                    if (amountNode != null)
                    {
                        if (decimal.TryParse(amountNode.InnerText, out decimal amount))
                        {
                            command.Parameters.AddWithValue("@Amount", amount);
                        }

                        var currencyAttr = amountNode.Attributes?["Ccy"];
                        if (currencyAttr != null && !string.IsNullOrEmpty(currencyAttr.Value))
                        {
                            command.Parameters.AddWithValue("@Currency", currencyAttr.Value);
                        }
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogWarning(ex, "Failed to extract amount/currency from XML payload");
                    // Continue without amount/currency
                }

                await connection.OpenAsync();
                await command.ExecuteNonQueryAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to store message locally");
                // Don't throw - this shouldn't fail the main operation
            }
        }
    }
}