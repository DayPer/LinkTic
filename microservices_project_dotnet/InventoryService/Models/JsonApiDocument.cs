// InventoryService/Models/JsonApiDocument.cs
// Este archivo define la estructura base para las respuestas y solicitudes JSON:API.
// Debe estar en la carpeta Models de cada servicio que lo utilice.
using System.Text.Json.Serialization;
using System.Collections.Generic; // Necesario para List<T>

namespace InventoryService.Models // ¡IMPORTANTE! Asegúrate de que este sea el namespace correcto para InventoryService
{
    // Clase base para un documento JSON:API (puede contener data o errors)
    public class JsonApiDocument<T>
    {
        [JsonPropertyName("data")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public T Data { get; set; }

        [JsonPropertyName("errors")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public List<JsonApiError> Errors { get; set; }

        [JsonPropertyName("links")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public JsonApiLinks Links { get; set; } // Para paginación

        public JsonApiDocument() { }

        public JsonApiDocument(T data)
        {
            Data = data;
        }

        public JsonApiDocument(List<JsonApiError> errors)
        {
            Errors = errors;
        }
    }

    // Representa el objeto 'data' dentro de un documento JSON:API para un solo recurso
    public class JsonApiResource<TAttributes>
    {
        [JsonPropertyName("type")]
        public string Type { get; set; }

        [JsonPropertyName("id")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string Id { get; set; }

        [JsonPropertyName("attributes")]
        public TAttributes Attributes { get; set; }

        public JsonApiResource() { }
        public JsonApiResource(string type, TAttributes attributes, string id = null)
        {
            Type = type;
            Attributes = attributes;
            Id = id;
        }
    }

    // Para representar una lista de recursos en el campo 'data'
    public class JsonApiResourceList<TAttributes>
    {
        [JsonPropertyName("data")]
        public List<JsonApiResource<TAttributes>> Data { get; set; }

        public JsonApiResourceList() { }
        public JsonApiResourceList(List<JsonApiResource<TAttributes>> data)
        {
            Data = data;
        }
    }

    // Para la estructura de errores
    public class JsonApiError
    {
        [JsonPropertyName("status")]
        public string Status { get; set; }

        [JsonPropertyName("title")]
        public string Title { get; set; }

        [JsonPropertyName("detail")]
        public string Detail { get; set; }

        [JsonPropertyName("source")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public JsonApiErrorSource Source { get; set; }

        public JsonApiError(string status, string title, string detail, string pointer = null)
        {
            Status = status;
            Title = title;
            Detail = detail;
            if (!string.IsNullOrEmpty(pointer))
            {
                Source = new JsonApiErrorSource { Pointer = pointer };
            }
        }
    }

    public class JsonApiErrorSource
    {
        [JsonPropertyName("pointer")]
        public string Pointer { get; set; }
    }

    // Para enlaces de paginación
    public class JsonApiLinks
    {
        [JsonPropertyName("self")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string Self { get; set; }

        [JsonPropertyName("first")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string First { get; set; }

        [JsonPropertyName("prev")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string Prev { get; set; }

        [JsonPropertyName("next")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string Next { get; set; }

        [JsonPropertyName("last")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string Last { get; set; }
    }
}
