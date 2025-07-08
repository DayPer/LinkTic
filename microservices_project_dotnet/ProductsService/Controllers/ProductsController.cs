// ProductsService/Controllers/ProductsController.cs
using Microsoft.AspNetCore.Mvc;
using ProductsService.Models; // Necesario para los modelos JSON:API y Product
using ProductsService.Services; // Necesario para IProductService
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProductsService.Controllers
{
    [ApiController]
    [Route("[controller]")] // La ruta base del controlador es /products
    public class ProductsController : ControllerBase
    {
        private readonly IProductService _productService; // Inyección de la interfaz del servicio de aplicación

        // Constructor que recibe la interfaz del servicio de aplicación.
        public ProductsController(IProductService productService)
        {
            _productService = productService;
        }

        // GET /products
        [HttpGet]
        public async Task<IActionResult> GetProducts([FromQuery(Name = "page[number]")] int pageNumber = 1, [FromQuery(Name = "page[size]")] int pageSize = 10)
        {
            if (pageNumber < 1 || pageSize < 1)
            {
                return BadRequest(new JsonApiDocument<object>
                {
                    Errors = new List<JsonApiError>
                    {
                        new JsonApiError { Status = "400", Title = "Bad Request", Detail = "Page number and page size must be positive." }
                    }
                });
            }

            // Llama al servicio de aplicación para obtener los productos
            var (productResources, links, totalCount) = await _productService.GetProductsAsync(
                pageNumber,
                pageSize,
                (num, size) => Url.Action(nameof(GetProducts), new { page_number = num, page_size = size })
            );

            return Ok(new JsonApiDocument<List<JsonApiResource<ProductAttributes>>>
            {
                Data = productResources,
                Links = links
            });
        }

        // GET /products/{id}
        [HttpGet("{id}")]
        public async Task<IActionResult> GetProduct(Guid id)
        {
            // Llama al servicio de aplicación para obtener un producto por ID
            var productResource = await _productService.GetProductByIdAsync(id);

            if (productResource == null)
            {
                return NotFound(new JsonApiDocument<object>
                {
                    Errors = new List<JsonApiError>
                    {
                        new JsonApiError { Status = "404", Title = "Not Found", Detail = $"Product with ID '{id}' not found." }
                    }
                });
            }

            return Ok(new JsonApiDocument<JsonApiResource<ProductAttributes>> { Data = productResource });
        }

        // POST /products
        [HttpPost]
        public async Task<IActionResult> CreateProduct([FromBody] JsonApiRequestDocument<ProductCreateAttributes> request)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors)
                                            .Select(e => new JsonApiError { Detail = e.ErrorMessage })
                                            .ToList();
                return BadRequest(new JsonApiDocument<object> { Errors = errors });
            }

            if (request == null || request.Data == null || request.Data.Attributes == null)
            {
                return BadRequest(new JsonApiDocument<object>
                {
                    Errors = new List<JsonApiError>
                    {
                        new JsonApiError { Status = "400", Title = "Bad Request", Detail = "Invalid JSON:API document structure." }
                    }
                });
            }

            // Llama al servicio de aplicación para crear el producto
            var productResource = await _productService.CreateProductAsync(request.Data.Attributes);

            return CreatedAtAction(nameof(GetProduct), new { id = Guid.Parse(productResource.Id!) }, new JsonApiDocument<JsonApiResource<ProductAttributes>> { Data = productResource });
        }

        // PATCH /products/{id}
        [HttpPatch("{id}")]
        public async Task<IActionResult> UpdateProduct(Guid id, [FromBody] JsonApiRequestDocument<ProductUpdateAttributes> request)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors)
                                            .Select(e => new JsonApiError { Detail = e.ErrorMessage })
                                            .ToList();
                return BadRequest(new JsonApiDocument<object> { Errors = errors });
            }

            if (request == null || request.Data == null || request.Data.Attributes == null)
            {
                return BadRequest(new JsonApiDocument<object>
                {
                    Errors = new List<JsonApiError>
                    {
                        new JsonApiError { Status = "400", Title = "Bad Request", Detail = "Invalid JSON:API document structure." }
                    }
                });
            }

            // JSON:API PATCH requiere que el ID en el cuerpo coincida con el de la URL si se proporciona
            if (request.Data.Id != null)
            {
                if (!Guid.TryParse(request.Data.Id, out Guid requestIdGuid) || requestIdGuid != id)
                {
                    return BadRequest(new JsonApiDocument<object>
                    {
                        Errors = new List<JsonApiError>
                        {
                            new JsonApiError { Status = "400", Title = "ID Mismatch", Detail = "The 'id' in the request body must match the 'id' in the URL." }
                        }
                    });
                }
            }

            // Llama al servicio de aplicación para actualizar el producto
            var productResource = await _productService.UpdateProductAsync(id, request.Data.Attributes);

            if (productResource == null)
            {
                return NotFound(new JsonApiDocument<object>
                {
                    Errors = new List<JsonApiError>
                    {
                        new JsonApiError { Status = "404", Title = "Not Found", Detail = $"Product with ID '{id}' not found." }
                    }
                });
            }

            return Ok(new JsonApiDocument<JsonApiResource<ProductAttributes>> { Data = productResource });
        }

        // DELETE /products/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProduct(Guid id)
        {
            // Llama al servicio de aplicación para eliminar el producto
            var deleted = await _productService.DeleteProductAsync(id);

            if (!deleted)
            {
                return NotFound(new JsonApiDocument<object>
                {
                    Errors = new List<JsonApiError>
                    {
                        new JsonApiError { Status = "404", Title = "Not Found", Detail = $"Product with ID '{id}' not found." }
                    }
                });
            }

            return NoContent(); // 204 No Content para eliminación exitosa
        }
    }
}
