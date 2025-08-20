//using BRGateway24.Models;
//using System.Net.Http.Headers;

//namespace BRGateway24.Repository.TISS
//{

//    public interface ITissClientService
//    {
//        Task<HttpResponseMessage> SendRequestAsync(
//            string endpoint,
//            HttpMethod method,
//            TissApiHeaders headers,
//            string content = null);
//    }

//    public class TissClientService : ITissClientService
//    {
//        private readonly HttpClient _httpClient;
//        private readonly ILogger<TissClientService> _logger;

//        public TissClientService(HttpClient httpClient, ILogger<TissClientService> logger)
//        {
//            _httpClient = httpClient;
//            _logger = logger;

//            _httpClient.BaseAddress = new Uri("https://196.46.101.90:8443/rtgs/");
//            _httpClient.DefaultRequestHeaders.Accept.Add(
//                new MediaTypeWithQualityHeaderValue("application/json"));
//        }

//        public async Task<HttpResponseMessage> SendRequestAsync(
//            string endpoint,
//            HttpMethod method,
//            TissApiHeaders headers,
//            string content = null)
//        {
//            try
//            {
//                var request = new HttpRequestMessage(method, endpoint);

//                // Add common headers
//                if (!string.IsNullOrEmpty(headers.Authorization))
//                {
//                    request.Headers.Add("Authorization", headers.Authorization);
//                }

//                // Add endpoint-specific headers
//                switch (endpoint.ToLower())
//                {
//                    case "businessdate":
//                    case "currenttimetableevent":
//                        request.Headers.Add("currency", headers.Currency);
//                        break;

//                    case "message":
//                        request.Headers.Add("payload_type", headers.PayloadType);
//                        request.Headers.Add("sender", headers.Sender);
//                        request.Headers.Add("consumer", headers.Consumer);
//                        request.Headers.Add("msgid", headers.MsgId);
//                        break;

//                    case "pendingtransactions":
//                    case "accountsactivity":
//                        request.Headers.Add("sender", headers.Sender);
//                        request.Headers.Add("currency", headers.Currency);
//                        request.Headers.Add("Authorization", headers.Authorization);
//                        break;
//                }

//                // Add content if present
//                if (content != null)
//                {
//                    request.Content = new StringContent(content);
//                    request.Content.Headers.ContentType = new MediaTypeHeaderValue(headers.ContentType);
//                }

//                // Log the request for debugging
//                _logger.LogInformation("Sending {Method} request to {Endpoint}", method, endpoint);
//                _logger.LogDebug("Headers: {Headers}", string.Join(", ", request.Headers.Select(h => $"{h.Key}: {string.Join(",", h.Value)}")));

//                // Bypass SSL certificate validation (remove in production)
//                var handler = new HttpClientHandler
//                {
//                    ServerCertificateCustomValidationCallback = (message, cert, chain, errors) => true
//                };

//                using var client = new HttpClient(handler);
//                client.BaseAddress = _httpClient.BaseAddress;
//                client.Timeout = TimeSpan.FromSeconds(30);

//                var response = await client.SendAsync(request);

//                // Log response for debugging
//                _logger.LogInformation("Received response: {StatusCode}", response.StatusCode);
//                var responseContent = await response.Content.ReadAsStringAsync();
//                _logger.LogDebug("Response content: {Content}", responseContent);

//                return response;
//            }
//            catch (Exception ex)
//            {
//                _logger.LogError(ex, "Error sending request to TISS server for endpoint: {Endpoint}", endpoint);
//                throw;
//            }
//        }
//    }
//}
