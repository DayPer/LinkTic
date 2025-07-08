// ProductsService/Services/ProductService.cs
using ProductsService.Models; // Necesario para los modelos de dominio y JSON:API
using ProductsService.Repositories; // Necesario para IProductRepository
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProductsService.Services
{
    // Implementación concreta del IProductService.
    // Contiene la lógica de negocio y utiliza el repositorio para el acceso a datos.
    public class ProductService : IProductService
    {
        private readonly IProductRepository _productRepository;

        // El constructor recibe una instancia de IProductRepository, inyectada por el contenedor de DI.
        public ProductService(IProductRepository productRepository)
        {
            _productRepository = productRepository;
        }

        public async Task<JsonApiResource<ProductAttributes>?> GetProductByIdAsync(Guid id)
        {
            var product = await _productRepository.GetByIdAsync(id);
            if (product == null)
            {
                return null;
            }
            return MapProductToJsonApiResource(product);
        }

        public async Task<(List<JsonApiResource<ProductAttributes>> products, JsonApiLinks links, int totalCount)> GetProductsAsync(int pageNumber, int pageSize, Func<int, int, string?> urlAction)
        {
            var (products, totalCount) = await _productRepository.GetAllAsync(pageNumber, pageSize);

            var productResources = products.Select(MapProductToJsonApiResource).ToList();

            var totalPages = (int)Math.Ceiling((double)totalCount / pageSize);

            var links = new JsonApiLinks
            {
                Self = urlAction(pageNumber, pageSize),
                First = urlAction(1, pageSize),
                Prev = pageNumber > 1 ? urlAction(pageNumber - 1, pageSize) : null,
                Next = pageNumber < totalPages ? urlAction(pageNumber + 1, pageSize) : null,
                Last = totalPages > 0 ? urlAction(totalPages, pageSize) : null
            };

            return (productResources, links, totalCount);
        }

        public async Task<JsonApiResource<ProductAttributes>> CreateProductAsync(ProductCreateAttributes attributes)
        {
            var product = new Product
            {
                Id = Guid.NewGuid(),
                Name = attributes.Name,
                Description = attributes.Description,
                Price = attributes.Price,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            var createdProduct = await _productRepository.AddAsync(product);
            return MapProductToJsonApiResource(createdProduct);
        }

        public async Task<JsonApiResource<ProductAttributes>?> UpdateProductAsync(Guid id, ProductUpdateAttributes attributes)
        {
            var existingProduct = await _productRepository.GetByIdAsync(id);
            if (existingProduct == null)
            {
                return null;
            }

            // Aplicar actualizaciones parciales
            if (attributes.Name != null)
            {
                existingProduct.Name = attributes.Name;
            }
            if (attributes.Description != null)
            {
                existingProduct.Description = attributes.Description;
            }
            if (attributes.Price.HasValue)
            {
                existingProduct.Price = attributes.Price.Value;
            }

            existingProduct.UpdatedAt = DateTime.UtcNow;

            await _productRepository.UpdateAsync(existingProduct);
            return MapProductToJsonApiResource(existingProduct);
        }

        public async Task<bool> DeleteProductAsync(Guid id)
        {
            var exists = await _productRepository.ExistsAsync(id);
            if (!exists)
            {
                return false;
            }
            await _productRepository.DeleteAsync(id);
            return true;
        }

        // Método helper para mapear la entidad Product a un recurso JSON:API
        private JsonApiResource<ProductAttributes> MapProductToJsonApiResource(Product product)
        {
            return new JsonApiResource<ProductAttributes>
            {
                Type = "products",
                Id = product.Id.ToString(),
                Attributes = new ProductAttributes
                {
                    Name = product.Name,
                    Description = product.Description,
                    Price = product.Price,
                    CreatedAt = product.CreatedAt,
                    UpdatedAt = product.UpdatedAt
                }
            };
        }
    }
}
