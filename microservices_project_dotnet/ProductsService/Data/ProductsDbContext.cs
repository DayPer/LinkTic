// ProductsService/Data/ProductsDbContext.cs
using Microsoft.EntityFrameworkCore;
using ProductsService.Models; // Necesario para la entidad Product

namespace ProductsService.Data
{
    // ProductsDbContext hereda de DbContext, la clase base de Entity Framework Core
    public class ProductsDbContext : DbContext
    {
        // Constructor que recibe las opciones del DbContext.
        // Estas opciones (incluyendo la cadena de conexión) son configuradas en Program.cs
        // y en ProductsDbContextFactory.cs para las herramientas de EF Core.
        public ProductsDbContext(DbContextOptions<ProductsDbContext> options) : base(options)
        {
        }

        // DbSet para la entidad Product.
        // Esto le dice a Entity Framework Core que hay una tabla 'Products' en la base de datos
        // que mapea a la clase 'Product'.
        public DbSet<Product> Products { get; set; }

        // Este método se usa para configurar el modelo de la base de datos.
        // Aquí puedes definir restricciones, relaciones, etc.
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configuración específica para la entidad Product:
            modelBuilder.Entity<Product>(entity =>
            {
                // Define la clave primaria. Por defecto, EF Core ya lo haría si la propiedad se llama 'Id'.
                entity.HasKey(e => e.Id);

                // Configura la propiedad Name como requerida y con una longitud máxima.
                entity.Property(e => e.Name)
                      .IsRequired()
                      .HasMaxLength(255);

                // Configura la propiedad Description con una longitud máxima.
                entity.Property(e => e.Description)
                      .HasMaxLength(1000);

                // Configura la propiedad Price con una precisión específica para valores monetarios.
                entity.Property(e => e.Price)
                      .HasColumnType("decimal(18,2)"); // Ejemplo: 18 dígitos en total, 2 después del punto decimal

                // Configura CreatedAt y UpdatedAt para que se establezcan automáticamente.
                // En un escenario real, esto podría manejarse a nivel de la aplicación o de la base de datos.
                // Para simplificar, los estamos manejando en el controlador.
                // entity.Property(e => e.CreatedAt).HasDefaultValueSql("GETUTCDATE()");
                // entity.Property(e => e.UpdatedAt).HasDefaultValueSql("GETUTCDATE()");
            });
        }
    }
}
