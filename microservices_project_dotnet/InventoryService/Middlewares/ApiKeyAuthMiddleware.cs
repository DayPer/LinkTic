// InventoryService/Middlewares/ApiKeyAuthMiddleware.cs
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using System.Threading.Tasks;
using InventoryService.Models;

namespace InventoryService.Middlewares
{
    public class ApiKeyAuthMiddleware
    {
        private readonly RequestDelegate _next;
        private const string APIKEYNAME = "X-API-Key";

        public ApiKeyAuthMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context, IConfiguration configuration)
        {
            if (!context.Request.Headers.TryGetValue(APIKEYNAME, out var extractedApiKey))
            {
                await _next(context); // Permitir si no hay API Key (ej. Swagger UI)
                return;
            }

            var inventoryApiKey = configuration.GetValue<string>("InventoryServiceApiKey");

            if (!inventoryApiKey.Equals(extractedApiKey))
            {
                context.Response.StatusCode = 401; // Unauthorized
                context.Response.ContentType = "application/vnd.api+json";
                var errors = new List<JsonApiError>
                {
                    new JsonApiError("401", "Unauthorized", "Invalid API Key provided.")
                };
                await context.Response.WriteAsJsonAsync(new JsonApiDocument<object>(errors));
                return;
            }

            await _next(context);
        }
    }
}
