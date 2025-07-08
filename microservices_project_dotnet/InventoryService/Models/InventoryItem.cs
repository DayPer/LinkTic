// InventoryService/Models/InventoryItem.cs
using System;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic; // Necesario para Dictionary

namespace InventoryService.Models
{
    public class InventoryItem
    {
        [Key] // Marca Id como la clave primaria para esta tabla de inventario
        public Guid Id { get; set; }

        [Required]
        public Guid ProductId { get; set; } // Clave foránea lógica al Product.Id en ProductsService

        [Range(0, int.MaxValue, ErrorMessage = "Quantity cannot be negative.")]
        public int Quantity { get; set; }

        public DateTime LastUpdated { get; set; } = DateTime.UtcNow; // Fecha de última actualización

        // Constructor sin parámetros para deserialización por EF Core
        public InventoryItem() { }

        // Constructor para facilitar la creación (opcional, útil para pruebas)
        public InventoryItem(Guid id, Guid productId, int quantity)
        {
            Id = id;
            ProductId = productId;
            Quantity = quantity;
        }

        // Método para formatear a JSON:API (puede permanecer igual)
        public object ToJsonApi(string productName = null)
        {
            var attributes = new Dictionary<string, object>
            {
                { "product_id", ProductId.ToString() }, // Convertir Guid a string para JSON:API
                { "quantity", Quantity },
                { "lastUpdated", LastUpdated }
            };

            if (!string.IsNullOrEmpty(productName))
            {
                attributes["product_name"] = productName;
            }

            return new
            {
                type = "inventories",
                id = ProductId.ToString(), // Usamos ProductId como ID del recurso de inventario en JSON:API
                attributes = attributes
            };
        }
    }
}
