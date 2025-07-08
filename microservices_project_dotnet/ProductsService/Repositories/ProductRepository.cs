// ProductsService/Repositories/ProductRepository.cs
using Microsoft.EntityFrameworkCore;
using ProductsService.Data; // Necesario para ProductsDbContext
using ProductsService.Models; // Necesario para la entidad Product
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProductsService.Repositories
{
    // Implementación concreta del IProductRepository usando Entity Framework Core.
    public class ProductRepository : IProductRepository
    {
        private readonly ProductsDbContext _context;

        // El constructor recibe una instancia de ProductsDbContext, inyectada por el contenedor de DI.
        public ProductRepository(ProductsDbContext context)
        {
            _context = context;
        }

        public async Task<Product?> GetByIdAsync(Guid id)
        {
            return await _context.Products.FindAsync(id);
        }

        public async Task<(List<Product> products, int totalCount)> GetAllAsync(int pageNumber, int pageSize)
        {
            var totalCount = await _context.Products.CountAsync();
            var products = await _context.Products
                                         .OrderBy(p => p.Name) // Ordenar para paginación consistente
                                         .Skip((pageNumber - 1) * pageSize)
                                         .Take(pageSize)
                                         .ToListAsync();
            return (products, totalCount);
        }

        public async Task<Product> AddAsync(Product product)
        {
            _context.Products.Add(product);
            await _context.SaveChangesAsync();
            return product;
        }

        public async Task UpdateAsync(Product product)
        {
            _context.Entry(product).State = EntityState.Modified;
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(Guid id)
        {
            var product = await _context.Products.FindAsync(id);
            if (product != null)
            {
                _context.Products.Remove(product);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<bool> ExistsAsync(Guid id)
        {
            return await _context.Products.AnyAsync(e => e.Id == id);
        }
    }
}
