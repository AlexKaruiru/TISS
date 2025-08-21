using BRGateway24.Models;
using BRGateway24.Repository.TISS.Mock;
using Microsoft.Extensions.Configuration;
using System.Net.Http.Headers;

namespace BRGateway24.Repository.TISS
{
    public interface ITissClientService
    {
        Task<HttpResponseMessage> SendRequestAsync(
            string endpoint,
            HttpMethod method,
            TissApiHeaders headers,
            string content = null);
    }

    public class TissClientService : ITissClientService
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<TissClientService> _logger;
        private readonly IConfiguration _configuration;
        private readonly IMockTissService _mockTissService;
        private readonly bool _useMockService;

        public TissClientService(
            HttpClient httpClient,
            ILogger<TissClientService> logger,
            IConfiguration configuration,
            IMockTissService mockTissService)
        {
            _httpClient = httpClient;
            _logger = logger;
            _configuration = configuration;
            _mockTissService = mockTissService;

            // Check if we should use mock service (when TISS server is not accessible)
            _useMockService = _configuration.GetValue<bool>("TISS:UseMockService", true);

            if (!_useMockService)
            {
                _httpClient.BaseAddress = new Uri("https://196.46.101.90:8443/rtgs/");
                _httpClient.DefaultRequestHeaders.Accept.Add(
                    new MediaTypeWithQualityHeaderValue("application/json"));
            }
        }

        public async Task<HttpResponseMessage> SendRequestAsync(
            string endpoint,
            HttpMethod method,
            TissApiHeaders headers,
            string content = null)
        {
            if (_useMockService)
            {
                return await HandleMockRequestAsync(endpoint, method, headers, content);
            }
            else
            {
                return await HandleRealRequestAsync(endpoint, method, headers, content);
            }
        }

        // Repository/TISS/TissClientService.cs
        private async Task<HttpResponseMessage> HandleMockRequestAsync(
            string endpoint,
            HttpMethod method,
            TissApiHeaders headers,
            string content)
        {
            _logger.LogInformation("Using mock TISS service for endpoint: {Endpoint}", endpoint);

            try
            {
                string responseContent;
                HttpResponseMessage response;

                // Extract the endpoint name from the full path
                var endpointName = GetEndpointName(endpoint);

                switch (endpointName.ToLower())
                {
                    case "businessdate":
                        responseContent = await _mockTissService.GetBusinessDateAsync(headers.Currency);
                        response = new HttpResponseMessage(System.Net.HttpStatusCode.OK)
                        {
                            Content = new StringContent(responseContent, System.Text.Encoding.UTF8, "application/json")
                        };
                        break;

                    case "currenttimetableevent":
                        responseContent = await _mockTissService.GetCurrentTimetableEventAsync(headers.Currency);
                        response = new HttpResponseMessage(System.Net.HttpStatusCode.OK)
                        {
                            Content = new StringContent(responseContent, System.Text.Encoding.UTF8, "application/json")
                        };
                        break;

                    case "message":
                        responseContent = await _mockTissService.PostMessageAsync(headers, content);
                        response = new HttpResponseMessage(System.Net.HttpStatusCode.OK)
                        {
                            Content = new StringContent(responseContent, System.Text.Encoding.UTF8, "text/xml")
                        };
                        break;

                    case "pendingtransactions":
                        responseContent = await _mockTissService.GetPendingTransactionsAsync(headers.Sender, headers.Currency, headers.Authorization);
                        response = new HttpResponseMessage(System.Net.HttpStatusCode.OK)
                        {
                            Content = new StringContent(responseContent, System.Text.Encoding.UTF8, "application/json")
                        };
                        break;

                    case "accountsactivity":
                        responseContent = await _mockTissService.GetAccountsActivityAsync(headers.Sender, headers.Currency, headers.Authorization);
                        response = new HttpResponseMessage(System.Net.HttpStatusCode.OK)
                        {
                            Content = new StringContent(responseContent, System.Text.Encoding.UTF8, "application/json")
                        };
                        break;

                    default:
                        response = new HttpResponseMessage(System.Net.HttpStatusCode.NotFound)
                        {
                            Content = new StringContent("Endpoint not found")
                        };
                        break;
                }

                return response;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Mock TISS service error for endpoint: {Endpoint}", endpoint);
                return new HttpResponseMessage(System.Net.HttpStatusCode.InternalServerError)
                {
                    Content = new StringContent($"Mock service error: {ex.Message}")
                };
            }
        }

        // Helper method to extract endpoint name from full path
        private string GetEndpointName(string fullEndpoint)
        {
            if (string.IsNullOrEmpty(fullEndpoint))
                return fullEndpoint;

            // Remove the base path and get just the endpoint name
            var parts = fullEndpoint.Split('/');
            return parts.LastOrDefault() ?? fullEndpoint;
        }

        private async Task<HttpResponseMessage> HandleRealRequestAsync(
            string endpoint,
            HttpMethod method,
            TissApiHeaders headers,
            string content)
        {
            _logger.LogInformation("Sending real request to TISS endpoint: {Endpoint}", endpoint);

            try
            {
                var request = new HttpRequestMessage(method, endpoint);

                // Add common headers
                if (!string.IsNullOrEmpty(headers.Authorization))
                {
                    request.Headers.Add("Authorization", headers.Authorization);
                }

                // Add endpoint-specific headers
                switch (endpoint.ToLower())
                {
                    case "businessdate":
                    case "currenttimetableevent":
                        request.Headers.Add("currency", headers.Currency);
                        break;

                    case "message":
                        request.Headers.Add("payload_type", headers.PayloadType);
                        request.Headers.Add("sender", headers.Sender);
                        request.Headers.Add("consumer", headers.Consumer);
                        request.Headers.Add("msgid", headers.MsgId);
                        break;

                    case "pendingtransactions":
                    case "accountsactivity":
                        request.Headers.Add("sender", headers.Sender);
                        request.Headers.Add("currency", headers.Currency);
                        request.Headers.Add("Authorization", headers.Authorization);
                        break;
                }

                // Add content if present
                if (content != null)
                {
                    request.Content = new StringContent(content);
                    request.Content.Headers.ContentType = new MediaTypeHeaderValue(headers.ContentType);
                }

                // Bypass SSL certificate validation (remove in production)
                var handler = new HttpClientHandler
                {
                    ServerCertificateCustomValidationCallback = (message, cert, chain, errors) => true
                };

                using var client = new HttpClient(handler);
                client.BaseAddress = _httpClient.BaseAddress;
                client.Timeout = TimeSpan.FromSeconds(30);

                return await client.SendAsync(request);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error sending request to TISS server for endpoint: {Endpoint}", endpoint);
                throw;
            }
        }
    }
}