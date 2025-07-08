// ProductsService/Middlewares/ApiKeyAuthMiddleware.cs
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using System.Threading.Tasks;
using System.Net; // Para HttpStatusCode

namespace ProductsService.Middlewares
{
    public class ApiKeyAuthMiddleware
    {
        private readonly RequestDelegate _next; // El siguiente middleware en la cadena
        private readonly IConfiguration _configuration; // Para acceder a la configuración (ej. appsettings.json)
        private const string APIKEYNAME = "X-Api-Key"; // Nombre del encabezado donde se espera la API Key

        public ApiKeyAuthMiddleware(RequestDelegate next, IConfiguration configuration)
        {
            _next = next;
            _configuration = configuration;
        }

        // Método InvokeAsync es donde se procesa la solicitud
        public async Task InvokeAsync(HttpContext context)
        {
            // 1. Intentar obtener la API Key del encabezado de la solicitud
            if (!context.Request.Headers.TryGetValue(APIKEYNAME, out var extractedApiKey))
            {
                // Si el encabezado no está presente, devolver 401 Unauthorized
                context.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
                await context.Response.WriteAsync("API Key missing.");
                return; // Detener el procesamiento de la solicitud
            }

            // 2. Obtener la API Key esperada de la configuración (appsettings.json)
            // Asegúrate de que esta clave exista en tu appsettings.json
            // Por ejemplo: "ApiKeys": { "ProductsService": "your_secret_api_key_here" }
            var apiKey = _configuration.GetValue<string>("ApiKeys:ProductsService");

            // 3. Validar la API Key
            if (string.IsNullOrEmpty(apiKey) || !apiKey.Equals(extractedApiKey))
            {
                // Si la API Key no coincide o es nula/vacía en la configuración, devolver 403 Forbidden
                context.Response.StatusCode = (int)HttpStatusCode.Forbidden;
                await context.Response.WriteAsync("Invalid API Key.");
                return; // Detener el procesamiento de la solicitud
            }

            // Si la API Key es válida, pasar la solicitud al siguiente middleware en la cadena
            await _next(context);
        }
    }
}
