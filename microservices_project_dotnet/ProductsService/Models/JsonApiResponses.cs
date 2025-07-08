// ProductsService/Models/JsonApiResponses.cs
// Este archivo contiene las definiciones genéricas para las respuestas JSON:API.

using System.Text.Json.Serialization;
using System.Collections.Generic; // Necesario para List<JsonApiError>
using System; // Necesario para DateTime y Guid si se usan en atributos

namespace ProductsService.Models
{
    // Clase base para un recurso JSON:API en una respuesta.
    // TAttributes representa el tipo de los atributos del recurso (ej. ProductAttributes).
    public class JsonApiResource<TAttributes> // <-- ¡ESTA ES LA ÚNICA DEFINICIÓN DE JsonApiResource!
    {
        [JsonPropertyName("type")]
        public string Type { get; set; } = string.Empty; // Tipo del recurso (ej. "products", "inventories")

        [JsonPropertyName("id")]
        public string? Id { get; set; } // ID del recurso (Guid como string)

        [JsonPropertyName("attributes")]
        public TAttributes Attributes { get; set; } = default!; // Atributos específicos del recurso
    }

    // Clase para un objeto de error JSON:API en una respuesta.
    public class JsonApiError
    {
        [JsonPropertyName("status")]
        public string? Status { get; set; } // Código de estado HTTP como string (ej. "400", "404")

        [JsonPropertyName("title")]
        public string? Title { get; set; } // Resumen breve del problema

        [JsonPropertyName("detail")]
        public string? Detail { get; set; } // Descripción más detallada del problema

        [JsonPropertyName("source")]
        public JsonApiErrorSource? Source { get; set; } // Información sobre la fuente del error
    }

    // Clase para la fuente de un error JSON:API.
    public class JsonApiErrorSource
    {
        [JsonPropertyName("pointer")]
        public string? Pointer { get; set; } // Puntero JSON a la parte del documento que causó el error
    }

    // Clase para los enlaces de paginación o relacionados en una respuesta.
    public class JsonApiLinks
    {
        [JsonPropertyName("self")]
        public string? Self { get; set; }

        [JsonPropertyName("first")]
        public string? First { get; set; }

        [JsonPropertyName("prev")]
        public string? Prev { get; set; }

        [JsonPropertyName("next")]
        public string? Next { get; set; }

        [JsonPropertyName("last")]
        public string? Last { get; set; }
    }

    // Clase genérica para un DOCUMENTO JSON:API de RESPUESTA.
    // TData puede ser un solo recurso (JsonApiResource<TAttributes>) o una lista (List<JsonApiResource<TAttributes>>).
    public class JsonApiDocument<TData> // <-- ¡ESTA ES LA ÚNICA DEFINICIÓN DE JsonApiDocument aquí!
    {
        [JsonPropertyName("data")]
        public TData? Data { get; set; }

        [JsonPropertyName("errors")]
        public List<JsonApiError>? Errors { get; set; }

        [JsonPropertyName("links")]
        public JsonApiLinks? Links { get; set; }
    }

    // ATRIBUTOS ESPECÍFICOS PARA PRODUCTOS (PARA RESPUESTAS)
    public class ProductAttributes
    {
        [JsonPropertyName("name")]
        public string Name { get; set; } = string.Empty;

        [JsonPropertyName("description")]
        public string Description { get; set; } = string.Empty;

        [JsonPropertyName("price")]
        public decimal Price { get; set; }

        [JsonPropertyName("createdAt")]
        public DateTime CreatedAt { get; set; }

        [JsonPropertyName("updatedAt")]
        public DateTime UpdatedAt { get; set; }
    }
}
