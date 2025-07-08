// InventoryService/Data/InventoryDbContext.cs
using Microsoft.EntityFrameworkCore;
using InventoryService.Models;
using Microsoft.EntityFrameworkCore.Metadata.Builders; // Mantener por si acaso para otras configuraciones
using System; // Necesario para Guid y DateTime
using System.Linq; // Necesario para LINQ en SaveChanges
using System.Threading; // Necesario para CancellationToken
using System.Threading.Tasks; // Necesario para SaveChangesAsync

namespace InventoryService.Data
{
    public class InventoryDbContext : DbContext
    {
        public InventoryDbContext(DbContextOptions<InventoryDbContext> options) : base(options) { }

        // DbSet representa la tabla 'InventoryItems' en tu base de datos
        public DbSet<InventoryItem> InventoryItems { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<InventoryItem>(entity =>
            {
                entity.HasKey(e => e.Id); // Define Id como clave primaria para InventoryItem
                entity.Property(e => e.Id).ValueGeneratedOnAdd(); // EF Core generará el GUID al añadir

                entity.Property(e => e.ProductId)
                      .IsRequired(); // ProductId es obligatorio

                entity.Property(e => e.Quantity)
                      .IsRequired(); // Cantidad es obligatoria

                // --- LÍNEA ELIMINADA: Ya no se usa HasDefaultValueSql aquí ---
                // entity.Property(e => e.LastUpdated)
                //       .HasDefaultValueSql("GETUTCDATE()"); 
                // -----------------------------------------------------------

                // Opcional: Si quieres que LastUpdated siempre se actualice en la BD,
                // puedes configurarlo como un valor generado por la BD en cada actualización.
                // Sin embargo, la forma más común es manejarlo en el código de la aplicación.
                // entity.Property(e => e.LastUpdated).ValueGeneratedOnAddOrUpdate();
            });
        }

        // --- ALTERNATIVA: Sobrescribir SaveChanges para actualizar LastUpdated ---
        // Esta es la forma más común de manejar CreatedAt/UpdatedAt cuando no se usa DefaultValueSql
        public override int SaveChanges()
        {
            UpdateTimestamps();
            return base.SaveChanges();
        }

        public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            UpdateTimestamps();
            return await base.SaveChangesAsync(cancellationToken);
        }

        private void UpdateTimestamps()
        {
            var entries = ChangeTracker
                .Entries()
                .Where(e => e.Entity is InventoryItem && (
                        e.State == EntityState.Added ||
                        e.State == EntityState.Modified));

            foreach (var entityEntry in entries)
            {
                ((InventoryItem)entityEntry.Entity).LastUpdated = DateTime.UtcNow;

                if (entityEntry.State == EntityState.Added)
                {
                    // Si también tuvieras un 'CreatedAt', lo establecerías aquí por primera vez
                    // ((InventoryItem)entityEntry.Entity).CreatedAt = DateTime.UtcNow;
                }
            }
        }
    }
}
