// InventoryService/Models/ProductApiModel.cs
using System.Text.Json.Serialization;

namespace InventoryService.Models
{
    // Modelo de atributos de producto del ProductsService
    public class ProductApiAttributes
    {
        [JsonPropertyName("name")]
        public string Name { get; set; }
        [JsonPropertyName("description")]
        public string Description { get; set; }
        [JsonPropertyName("price")]
        public decimal Price { get; set; }
    }

    // Modelo de recurso de producto del ProductsService
    public class ProductApiResource : JsonApiResource<ProductApiAttributes> { }

    // Modelo de documento JSON:API de producto del ProductsService
    public class ProductApiDocument : JsonApiDocument<ProductApiResource> { }
}