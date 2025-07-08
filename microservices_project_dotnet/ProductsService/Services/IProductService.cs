// ProductsService/Services/IProductService.cs
using ProductsService.Models; // Necesario para Product, ProductAttributes, ProductCreateAttributes, ProductUpdateAttributes
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ProductsService.Services
{
    // Interfaz que define las operaciones de negocio para los productos.
    // Esta interfaz es consumida por los controladores.
    public interface IProductService
    {
        Task<JsonApiResource<ProductAttributes>?> GetProductByIdAsync(Guid id);
        Task<(List<JsonApiResource<ProductAttributes>> products, JsonApiLinks links, int totalCount)> GetProductsAsync(int pageNumber, int pageSize, Func<int, int, string?> urlAction);
        Task<JsonApiResource<ProductAttributes>> CreateProductAsync(ProductCreateAttributes attributes);
        Task<JsonApiResource<ProductAttributes>?> UpdateProductAsync(Guid id, ProductUpdateAttributes attributes);
        Task<bool> DeleteProductAsync(Guid id);
    }
}
