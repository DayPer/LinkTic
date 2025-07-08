// ProductsService/Repositories/IProductRepository.cs
using ProductsService.Models; // Necesario para la entidad Product
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ProductsService.Repositories
{
    // Interfaz que define las operaciones de acceso a datos para la entidad Product.
    // Esto permite la inyección de dependencias y facilita la testabilidad (mocking).
    public interface IProductRepository
    {
        Task<Product?> GetByIdAsync(Guid id); // Obtener un producto por su ID
        Task<(List<Product> products, int totalCount)> GetAllAsync(int pageNumber, int pageSize); // Obtener todos los productos con paginación
        Task<Product> AddAsync(Product product); // Añadir un nuevo producto
        Task UpdateAsync(Product product); // Actualizar un producto existente
        Task DeleteAsync(Guid id); // Eliminar un producto por su ID
        Task<bool> ExistsAsync(Guid id); // Verificar si un producto existe
    }
}
