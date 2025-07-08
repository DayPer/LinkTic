// ProductsService/Models/Product.cs
// Este archivo contiene la definición de la entidad de base de datos 'Product'.

using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema; // Necesario para [Column]

namespace ProductsService.Models
{
    public class Product
    {
        [Key] // Define Id como clave primaria
        public Guid Id { get; set; }

        [Required] // Nombre es obligatorio
        [MaxLength(255)] // Longitud máxima para el nombre
        public string Name { get; set; } = string.Empty;

        [MaxLength(1000)] // Longitud máxima para la descripción
        public string Description { get; set; } = string.Empty;

        [Column(TypeName = "decimal(18,2)")] // Define el tipo de columna para precisión de dinero
        public decimal Price { get; set; }

        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}
