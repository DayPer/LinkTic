// InventoryService/Data/InventoryDbContextFactory.cs
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using System.IO;
using System;

namespace InventoryService.Data
{
    public class InventoryDbContextFactory : IDesignTimeDbContextFactory<InventoryDbContext>
    {
        public InventoryDbContext CreateDbContext(string[] args)
        {
            Console.WriteLine("Attempting to create InventoryDbContext for design time...");

            var currentDirectory = Directory.GetCurrentDirectory();
            Console.WriteLine($"Current Directory for appsettings.json: {currentDirectory}");

            var configuration = new ConfigurationBuilder()
                .SetBasePath(currentDirectory)
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .Build();

            // Obtenemos la cadena de conexión original del appsettings.json
            var originalConnectionString = configuration.GetConnectionString("InventoryDbConnection");

            if (string.IsNullOrEmpty(originalConnectionString))
            {
                Console.WriteLine("ERROR: InventoryDbConnection connection string not found or is empty!");
                throw new InvalidOperationException("InventoryDbConnection connection string not found in appsettings.json. Make sure the file exists and the connection string is defined.");
            }

            // Modificamos la cadena de conexión para usar 'localhost' en lugar de 'mssql-db'.
            // Esto es crucial porque las herramientas 'dotnet ef' se ejecutan desde el host (tu máquina),
            // no desde un contenedor de Docker en la misma red que 'mssql-db'.
            var connectionStringForMigrations = originalConnectionString.Replace("Server=mssql-db", "Server=localhost");
            Console.WriteLine($"Connection string loaded for migrations: {connectionStringForMigrations}");

            var builder = new DbContextOptionsBuilder<InventoryDbContext>();
            builder.UseSqlServer(connectionStringForMigrations, // Usamos la cadena de conexión modificada
                sqlServerOptionsAction: sqlOptions =>
                {
                    // Habilita la resiliencia a errores transitorios (reintentos automáticos de conexión)
                    sqlOptions.EnableRetryOnFailure(
                        maxRetryCount: 5, // Número máximo de reintentos
                        maxRetryDelay: TimeSpan.FromSeconds(30), // Retraso máximo entre reintentos
                        errorNumbersToAdd: null // Deja null para usar los códigos de error por defecto de SQL Server
                    );
                }
            );

            Console.WriteLine("InventoryDbContext created successfully for design time.");
            return new InventoryDbContext(builder.Options);
        }
    }
}
