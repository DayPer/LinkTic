// ProductsService/Data/ProductsDbContextFactory.cs
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using System.IO;

namespace ProductsService.Data
{
    // Implementa IDesignTimeDbContextFactory para permitir que las herramientas de EF Core
    // creen una instancia de ProductsDbContext en tiempo de diseño (ej. para migraciones).
    public class ProductsDbContextFactory : IDesignTimeDbContextFactory<ProductsDbContext>
    {
        public ProductsDbContext CreateDbContext(string[] args)
        {
            // Construye la configuración para leer la cadena de conexión desde appsettings.json
            // Esto simula cómo la aplicación lee la configuración en tiempo de ejecución.
            var configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory()) // Establece la ruta base al directorio actual del proyecto
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true) // Carga appsettings.json
                .AddJsonFile("appsettings.Development.json", optional: true, reloadOnChange: true) // Carga appsettings.Development.json (para desarrollo)
                .Build();

            // Obtiene la cadena de conexión
            var connectionString = configuration.GetConnectionString("ProductsDbConnection");

            // Configura DbContextOptions para ProductsDbContext
            var builder = new DbContextOptionsBuilder<ProductsDbContext>();
            builder.UseSqlServer(connectionString);

            // Retorna una nueva instancia de ProductsDbContext con las opciones configuradas
            return new ProductsDbContext(builder.Options);
        }
    }
}
