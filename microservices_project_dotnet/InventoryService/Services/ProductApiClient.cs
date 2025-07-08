// InventoryService/Services/ProductApiClient.cs
using InventoryService.Models;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging; // Necesario para ILogger
using System; // Necesario para Guid
using System.Linq; // Necesario para .Any() en errores
using System.Net; // Necesario para HttpStatusCode

namespace InventoryService.Services
{
    public class ProductApiClient
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<ProductApiClient> _logger;

        public ProductApiClient(HttpClient httpClient, ILogger<ProductApiClient> logger)
        {
            _httpClient = httpClient;
            _logger = logger;
        }

        public async Task<ProductApiResource> GetProductByIdAsync(string productId)
        {
            Guid productIdGuid;
            if (!Guid.TryParse(productId, out productIdGuid))
            {
                _logger.LogWarning($"Attempted to get product with invalid GUID format: {productId}");
                return null; // O lanzar una excepción específica si prefieres
            }

            _logger.LogInformation($"Calling Products Service for product ID: {productId}");
            var response = await _httpClient.GetAsync($"/products/{productId}");

            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                var jsonApiDocument = JsonSerializer.Deserialize<ProductApiDocument>(content, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                if (jsonApiDocument?.Data != null && jsonApiDocument.Data.Type == "products")
                {
                    _logger.LogInformation($"Successfully retrieved product {productId} from Products Service.");
                    return jsonApiDocument.Data;
                }
                else if (jsonApiDocument?.Errors != null && jsonApiDocument.Errors.Any())
                {
                    _logger.LogError($"Product Service returned errors for {productId}: {string.Join(", ", jsonApiDocument.Errors.Select(e => e.Detail))}");
                    return null;
                }
                else
                {
                    _logger.LogWarning($"Product Service returned success but with unexpected JSON:API structure for {productId}.");
                    return null;
                }
            }
            else if (response.StatusCode == HttpStatusCode.NotFound)
            {
                _logger.LogWarning($"Product {productId} not found in Products Service (404).");
                return null;
            }
            else
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                _logger.LogError($"Error calling Products Service for {productId}. Status: {response.StatusCode}, Content: {errorContent}");
                response.EnsureSuccessStatusCode();
                return null;
            }
        }
    }
}
