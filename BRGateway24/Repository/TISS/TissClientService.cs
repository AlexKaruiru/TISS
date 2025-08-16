using BRGateway24.Models;
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

        public TissClientService(HttpClient httpClient, ILogger<TissClientService> logger)
        {
            _httpClient = httpClient;
            _logger = logger;

            // Configure base address for TISS server
            _httpClient.BaseAddress = new Uri("https://196.46.101.90:8443/rtgs/");

            // Configure default headers if needed
            _httpClient.DefaultRequestHeaders.Accept.Add(
                new MediaTypeWithQualityHeaderValue("application/json"));
        }

        public async Task<HttpResponseMessage> SendRequestAsync(
            string endpoint,
            HttpMethod method,
            TissApiHeaders headers,
            string content = null)
        {
            try
            {
                var request = new HttpRequestMessage(method, endpoint);

                // Add required headers
                request.Headers.Add("Authorization", headers.Authorization);
                request.Headers.Add("sender", headers.Sender);
                request.Headers.Add("msgid", headers.MsgId);
                request.Headers.Add("consumer", headers.Consumer);

                // Add content if present
                if (content != null)
                {
                    request.Content = new StringContent(content);

                    // Set content type based on headers
                    var contentType = headers.ContentType ?? "application/json";
                    request.Content.Headers.ContentType = new MediaTypeHeaderValue(contentType);
                }

                // Bypass SSL certificate validation for testing
                var handler = new HttpClientHandler
                {
                    ServerCertificateCustomValidationCallback =
                        HttpClientHandler.DangerousAcceptAnyServerCertificateValidator
                };

                using var client = new HttpClient(handler);
                client.BaseAddress = _httpClient.BaseAddress;

                return await client.SendAsync(request);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error sending request to TISS server");
                throw;
            }
        }
    }
}
