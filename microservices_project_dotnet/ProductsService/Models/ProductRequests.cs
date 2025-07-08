// ProductsService/Models/ProductRequests.cs
// Este archivo contiene las definiciones para las solicitudes JSON:API y los atributos específicos.

using System;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace ProductsService.Models // Asegúrate de que el namespace sea EXACTAMENTE este
{
    // Clase para los atributos de un producto cuando se crea (POST)
    public class ProductCreateAttributes
    {
        [Required(ErrorMessage = "Product name is required.")]
        [StringLength(255, ErrorMessage = "Product name cannot exceed 255 characters.")]
        public string Name { get; set; } = string.Empty;

        [StringLength(1000, ErrorMessage = "Product description cannot exceed 1000 characters.")]
        public string Description { get; set; } = string.Empty;

        [Range(0.01, 999999.99, ErrorMessage = "Price must be a positive value.")]
        public decimal Price { get; set; }
    }

    // Clase para los atributos de un producto cuando se actualiza (PATCH)
    // Las propiedades son opcionales (nullable) para permitir actualizaciones parciales.
    public class ProductUpdateAttributes
    {
        [StringLength(255, ErrorMessage = "Product name cannot exceed 255 characters.")]
        public string? Name { get; set; }

        [StringLength(1000, ErrorMessage = "Product description cannot exceed 1000 characters.")]
        public string? Description { get; set; }

        [Range(0.01, 999999.99, ErrorMessage = "Price must be a positive value.")]
        public decimal? Price { get; set; } // Usar decimal? para que sea opcional
    }

    // Clase genérica para la estructura de datos 'data' dentro de un documento JSON:API para SOLICITUDES.
    // TAttributes es el tipo de los atributos específicos (ej. ProductCreateAttributes, ProductUpdateAttributes).
    public class JsonApiRequestData<TAttributes>
    {
        [JsonPropertyName("type")]
        public string Type { get; set; } = string.Empty;

        [JsonPropertyName("id")]
        public string? Id { get; set; } // Opcional para POST (creación), requerido para PATCH/PUT (actualización)

        [JsonPropertyName("attributes")]
        public TAttributes Attributes { get; set; } = default!; // Los atributos específicos de la solicitud
    }

    // Clase genérica para un DOCUMENTO JSON:API de SOLICITUD (POST, PATCH).
    public class JsonApiRequestDocument<TAttributes>
    {
        [JsonPropertyName("data")]
        public JsonApiRequestData<TAttributes> Data { get; set; } = default!; // La sección 'data' de la solicitud
    }
}
