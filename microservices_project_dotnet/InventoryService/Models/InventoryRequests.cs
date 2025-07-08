// InventoryService/Models/InventoryRequests.cs
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace InventoryService.Models
{
    public class InventoryUpdateAttributes
    {
        [Required(ErrorMessage = "Quantity change is required.")]
        public int QuantityChange { get; set; } // Puede ser positivo o negativo
    }

    public class InventoryUpdateRequest : JsonApiDocument<JsonApiResource<InventoryUpdateAttributes>> { }

    public class InventoryResponse : JsonApiDocument<object> { }
}