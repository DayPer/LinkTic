// InventoryService/Controllers/InventoryController.cs
using Microsoft.AspNetCore.Mvc;
using InventoryService.Models;
using InventoryService.Services;
using InventoryService.Data; // SOLO para InventoryDbContext
using System.Net;
using System.Text.RegularExpressions;
using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging; // Necesario para ILogger
using System.Collections.Generic; // Necesario para List<T>
using System.Linq; // Necesario para LINQ

namespace InventoryService.Controllers
{
    [ApiController]
    [Route("inventories")]
    [Consumes("application/vnd.api+json")]
    [Produces("application/vnd.api+json")]
    public class InventoryController : ControllerBase
    {
        private readonly ProductApiClient _productApiClient;
        private readonly InventoryDbContext _dbContext; // Inyectamos el contexto de la base de datos del INVENTARIO
        private readonly ILogger<InventoryController> _logger;

        public InventoryController(ProductApiClient productApiClient, InventoryDbContext dbContext, ILogger<InventoryController> logger)
        {
            _productApiClient = productApiClient;
            _dbContext = dbContext;
            _logger = logger;
        }

        // Helper para generar errores JSON:API
        private IActionResult JsonApiError(HttpStatusCode statusCode, string title, string detail, string pointer = null)
        {
            Response.Headers.Add("Content-Type", "application/vnd.api+json");
            var error = new JsonApiError(((int)statusCode).ToString(), title, detail, pointer);
            return StatusCode((int)statusCode, new JsonApiDocument<object>(new List<JsonApiError> { error }));
        }

        /// <summary>
        /// Consults the quantity of a product in inventory.
        /// </summary>
        [HttpGet("{productId}")]
        [ProducesResponseType(typeof(InventoryResponse), 200)]
        [ProducesResponseType(typeof(JsonApiDocument<object>), 400)]
        [ProducesResponseType(typeof(JsonApiDocument<object>), 404)]
        [ProducesResponseType(typeof(JsonApiDocument<object>), 500)]
        [ProducesResponseType(typeof(JsonApiDocument<object>), 503)]
        public async Task<IActionResult> GetInventory(string productId)
        {
            Guid productIdGuid;
            if (!Guid.TryParse(productId, out productIdGuid))
            {
                return JsonApiError(HttpStatusCode.BadRequest, "Invalid ID Format", $"Product ID '{productId}' is not a valid GUID format.");
            }

            ProductApiResource product = null;
            try
            {
                // Llama al servicio de productos a través de su API HTTP
                product = await _productApiClient.GetProductByIdAsync(productId);
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError($"Error calling Product Service for ID {productId}: {ex.Message}");
                return JsonApiError(HttpStatusCode.InternalServerError, "Product Service Error", $"Failed to retrieve product details: {ex.Message}");
            }
            catch (Exception ex)
            {
                _logger.LogError($"An error occurred while calling Product Service for ID {productId}: {ex.Message}");
                return JsonApiError(HttpStatusCode.ServiceUnavailable, "Service Unavailable", $"Could not connect to Products Service: {ex.Message}");
            }

            if (product == null)
            {
                return JsonApiError(HttpStatusCode.NotFound, "Product Not Found", $"Product with ID '{productId}' not found in Products service.");
            }

            // Busca el item de inventario en la base de datos del INVENTARIO
            var inventoryItem = await _dbContext.InventoryItems
                                                .FirstOrDefaultAsync(i => i.ProductId == productIdGuid);

            // Si no existe, lo inicializa con cantidad 0 y lo añade a la BD del INVENTARIO
            if (inventoryItem == null)
            {
                inventoryItem = new InventoryItem { Id = Guid.NewGuid(), ProductId = productIdGuid, Quantity = 0 };
                _dbContext.InventoryItems.Add(inventoryItem);
                await _dbContext.SaveChangesAsync();
                _logger.LogInformation($"Log Event: Initial inventory record created for Product ID {productIdGuid} with quantity 0.");
            }

            return Ok(new InventoryResponse { Data = inventoryItem.ToJsonApi(product.Attributes.Name) });
        }

        /// <summary>
        /// Updates the available quantity of a product after a purchase or return.
        /// </summary>
        [HttpPatch("{productId}")]
        [ProducesResponseType(typeof(InventoryResponse), 200)]
        [ProducesResponseType(typeof(JsonApiDocument<object>), 400)]
        [ProducesResponseType(typeof(JsonApiDocument<object>), 404)]
        [ProducesResponseType(typeof(JsonApiDocument<object>), 500)]
        [ProducesResponseType(typeof(JsonApiDocument<object>), 503)]
        public async Task<IActionResult> UpdateInventory(string productId, [FromBody] InventoryUpdateRequest request)
        {
            Guid productIdGuid;
            if (!Guid.TryParse(productId, out productIdGuid))
            {
                return JsonApiError(HttpStatusCode.BadRequest, "Invalid ID Format", $"Product ID '{productId}' in URL is not a valid GUID format.");
            }

            if (!ModelState.IsValid)
            {
                var errors = ModelState.Where(e => e.Value.Errors.Any())
                                    .SelectMany(e => e.Value.Errors.Select(er =>
                                        new JsonApiError("400", "Validation Error", er.ErrorMessage, Regex.Replace(e.Key, @"^[^.]*\.", "data/attributes/").Replace(".", "/"))))
                                    .ToList();
                return JsonApiError(HttpStatusCode.BadRequest, "Validation Failed", "One or more validation errors occurred.", errors.FirstOrDefault()?.Source?.Pointer);
            }

            // 1. Verifica que el producto exista en el servicio de productos
            ProductApiResource product = null;
            try
            {
                product = await _productApiClient.GetProductByIdAsync(productId);
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError($"Error calling Product Service for ID {productId}: {ex.Message}");
                return JsonApiError(HttpStatusCode.InternalServerError, "Product Service Error", $"Failed to retrieve product details: {ex.Message}");
            }
            catch (Exception ex)
            {
                _logger.LogError($"An error occurred while calling Product Service for ID {productId}: {ex.Message}");
                return JsonApiError(HttpStatusCode.ServiceUnavailable, "Service Unavailable", $"Could not connect to Products Service: {ex.Message}");
            }

            if (product == null)
            {
                return JsonApiError(HttpStatusCode.NotFound, "Product Not Found", $"Product with ID '{productId}' not found in Products service.");
            }

            var data = request.Data;
            if (data == null || data.Type != "inventories" || data.Attributes == null)
            {
                return JsonApiError(HttpStatusCode.BadRequest, "Invalid JSON:API Structure", "Expected a JSON:API 'data' object with 'type' as 'inventories' and 'attributes'.");
            }

            // Validar que el ID del cuerpo coincida con el de la URL
            if (data.Id != null)
            {
                Guid requestIdGuid;
                if (!Guid.TryParse(data.Id, out requestIdGuid))
                {
                    return JsonApiError(HttpStatusCode.BadRequest, "Invalid ID Format", $"ID in request body '{data.Id}' is not a valid GUID format.");
                }
                if (requestIdGuid != productIdGuid)
                {
                    return JsonApiError(HttpStatusCode.BadRequest, "ID Mismatch", "The 'id' in the request body must match the 'id' in the URL.");
                }
            }

            var quantityChange = data.Attributes.QuantityChange;

            // Busca el item de inventario existente o crea uno nuevo si no existe en la base de datos del INVENTARIO
            var inventoryItem = await _dbContext.InventoryItems
                                                .FirstOrDefaultAsync(i => i.ProductId == productIdGuid);

            int currentQuantity = 0;
            if (inventoryItem == null)
            {
                // Si no existe, crea un nuevo registro de inventario con cantidad 0
                inventoryItem = new InventoryItem { Id = Guid.NewGuid(), ProductId = productIdGuid, Quantity = 0 };
                _dbContext.InventoryItems.Add(inventoryItem);
                _logger.LogInformation($"Log Event: Initial inventory record created for Product ID {productIdGuid} with quantity 0 before update.");
            }
            else
            {
                currentQuantity = inventoryItem.Quantity;
            }

            var newQuantity = currentQuantity + quantityChange;

            if (newQuantity < 0)
            {
                return JsonApiError(HttpStatusCode.BadRequest, "Insufficient Stock", $"Insufficient stock. Current: {currentQuantity}, Requested change: {quantityChange}.");
            }

            inventoryItem.Quantity = newQuantity;
            inventoryItem.LastUpdated = DateTime.UtcNow;

            _dbContext.InventoryItems.Update(inventoryItem);
            await _dbContext.SaveChangesAsync();

            _logger.LogInformation($"Log Event: Inventory updated for Product ID {productIdGuid}: Old quantity {currentQuantity}, Change {quantityChange}, New quantity {newQuantity}");

            return Ok(new InventoryResponse { Data = inventoryItem.ToJsonApi(product.Attributes.Name) });
        }
    }
}
