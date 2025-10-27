using System.Net.Http.Headers;
using System.Text;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace FiapChallenge.Web.Services
{
    public class ApiService
    {
        private readonly HttpClient _httpClient;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ILogger<ApiService> _logger;

        public ApiService(HttpClient httpClient, IHttpContextAccessor httpContextAccessor, ILogger<ApiService> logger)
        {
            _httpClient = httpClient;
            _httpContextAccessor = httpContextAccessor;
            _logger = logger;

            _logger.LogInformation("=== ApiService initialized with BaseAddress: '{BaseAddress}' ===", _httpClient.BaseAddress);
        }


        private string NormalizeEndpoint(string endpoint)
        {
            return endpoint.TrimStart('/');
        }

        private void ConfigureAuthorizationHeader(string? token)
        {
            // Clear any existing Authorization header
            _httpClient.DefaultRequestHeaders.Authorization = null;

            if (!string.IsNullOrEmpty(token))
            {
                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                _logger.LogInformation("=== Authorization header SET: Bearer {TokenPreview}... ===", token.Substring(0, Math.Min(30, token.Length)));
            }
            else
            {
                _logger.LogWarning("=== NO TOKEN PROVIDED - Authorization header NOT set ===");
            }
        }

        public async Task<T?> GetAsync<T>(string endpoint, string? token = null)
        {
            _logger.LogInformation("=== GetAsync called for endpoint: '{Endpoint}' ===", endpoint);
            _logger.LogInformation("Token parameter provided: {TokenProvided}", !string.IsNullOrEmpty(token));

            var normalizedEndpoint = NormalizeEndpoint(endpoint);
            var fullUrl = $"{_httpClient.BaseAddress}{normalizedEndpoint}";
            _logger.LogInformation("=== Full URL: '{FullUrl}' ===", fullUrl);

            ConfigureAuthorizationHeader(token);

            var response = await _httpClient.GetAsync(normalizedEndpoint);

            _logger.LogInformation("=== Response Status: {StatusCode} ===", response.StatusCode);

            if (!response.IsSuccessStatusCode)
                return default;

            var content = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<T>(content);
        }

        public async Task<HttpResponseMessage> PostAsync<T>(string endpoint, T data, string? token = null)
        {
            var normalizedEndpoint = NormalizeEndpoint(endpoint);

            ConfigureAuthorizationHeader(token);

            var json = JsonConvert.SerializeObject(data);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            return await _httpClient.PostAsync(normalizedEndpoint, content);
        }

        public async Task<HttpResponseMessage> PutAsync<T>(string endpoint, T data, string? token = null)
        {
            var normalizedEndpoint = NormalizeEndpoint(endpoint);

            ConfigureAuthorizationHeader(token);

            var json = JsonConvert.SerializeObject(data);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            return await _httpClient.PutAsync(normalizedEndpoint, content);
        }

        public async Task<HttpResponseMessage> DeleteAsync(string endpoint, string? token = null)
        {
            var normalizedEndpoint = NormalizeEndpoint(endpoint);

            ConfigureAuthorizationHeader(token);

            return await _httpClient.DeleteAsync(normalizedEndpoint);
        }
    }
}
