// ProductsService/Program.cs
using ProductsService.Middlewares; // Si tienes middlewares personalizados
using Microsoft.OpenApi.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using ProductsService.Data; // Necesario para ProductsDbContext
using ProductsService.Repositories; // Necesario para IProductRepository y ProductRepository
using ProductsService.Services; // Necesario para IProductService y ProductService
using Microsoft.EntityFrameworkCore; // Necesario para UseSqlServer
using System;

var builder = WebApplication.CreateBuilder(args);

// Define una política de CORS
var MyAllowSpecificOrigins = "_myAllowSpecificOrigins";

// Configura CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy(name: MyAllowSpecificOrigins,
                      builder =>
                      {
                          // Permite solicitudes desde tu frontend Angular (ajusta el puerto si es diferente)
                          builder.WithOrigins("http://localhost:4200", "http://localhost:51081")
                                 .AllowAnyHeader() // Permite cualquier encabezado en la solicitud
                                 .AllowAnyMethod(); // Permite cualquier método HTTP (GET, POST, PUT, PATCH, DELETE)
                      });
});

// Add services to the container.
builder.Services.AddControllers()
    .AddNewtonsoftJson(options =>
    {
        // Configuraciones de serialización si es necesario
    })
    .ConfigureApiBehaviorOptions(options =>
    {
        options.SuppressModelStateInvalidFilter = true;
    });

// Configuración de la base de datos con Entity Framework Core
builder.Services.AddDbContext<ProductsDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("ProductsDbConnection"),
        sqlServerOptionsAction: sqlOptions =>
        {
            sqlOptions.EnableRetryOnFailure(
                maxRetryCount: 5, // Número máximo de reintentos
                maxRetryDelay: TimeSpan.FromSeconds(30), // Retraso máximo entre reintentos
                errorNumbersToAdd: null // Añade códigos de error SQL adicionales si es necesario
            );
        }
    ));

// ¡NUEVAS REGISTRACIONES DE SERVICIOS PARA LA ARQUITECTURA POR CAPAS!
builder.Services.AddScoped<IProductRepository, ProductRepository>(); // Registra el repositorio
builder.Services.AddScoped<IProductService, ProductsService.Services.ProductService>(); // Registra el servicio de aplicación

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Products API", Version = "v1" });
});

var app = builder.Build();

// Usa la política de CORS
app.UseCors(MyAllowSpecificOrigins); // <-- ¡Añade esta línea ANTES de UseAuthorization y MapControllers!

app.UseAuthorization();

app.MapControllers();

// Configuración de Swagger UI
app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Products API V1");
    c.RoutePrefix = string.Empty; // Hace que Swagger UI sea accesible en la raíz del servicio (ej. http://localhost:5205/)
});

app.Run();
