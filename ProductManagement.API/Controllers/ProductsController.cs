using MediatR;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;
using ProductManagement.API.DTOs.Commands;
using ProductManagement.API.DTOs.Queries;
using ProductManagement.Infra.Data.MongoDB.Contexts;

namespace ProductManagement.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class ProductsController(IMediator mediator, MongoDbContext mongoDbContext) : ControllerBase
{
    [HttpPost]
    [ProducesResponseType(typeof(ProductResponseDto), 201)]
    public async Task<IActionResult> CreateProduct([FromBody] CreateProductDto dto)
    {
        var id = await mediator.Send(dto); //COMMAND
        return StatusCode(201, new
        {
            message = "Produto cadastrado com sucesso.",
            productId = id,
            data = dto
        });
    }

    [HttpPut("{id:guid}")]
    [ProducesResponseType(typeof(ProductResponseDto), 200)]
    public async Task<IActionResult> UpdateProduct(Guid id, [FromBody] UpdateProductDto dto)
    {
        dto.Id = id; //Capturando o Id do path (URI)
        await mediator.Send(dto); //COMMAND
        return Ok(new
        {
            message = "Produto atualizado com sucesso.",
            productId = id,
            data = dto
        });
    }

    [HttpPut("{id:guid}/stock")]
    [ProducesResponseType(typeof(ProductStockResponseDto), 200)]
    public async Task<IActionResult> UpdateStock(Guid id, [FromBody] UpdateStockDto dto)
    {
        dto.Id = id; //Capturando o Id do path (URI)
        await mediator.Send(dto); //COMMAND
        return Ok(new
        {
            message = "Estoque atualizado com sucesso.",
            productId = id,
            data = dto
        });
    }

    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(ProductResponseDto), 200)]
    public async Task<IActionResult> GetProductById(Guid id)
    {
        var product = await mongoDbContext.Products
                                .Find(p => p.Id == id.ToString())
                                .FirstOrDefaultAsync();

        if (product == null)
            return NotFound(new { message = "Produto não encontrado." });

        return Ok(new ProductResponseDto
        {
            Id = Guid.Parse(product.Id!),
            Name = product.Name!,
            Description = product.Description!,
            Price = product.Price!.Value
        });
    }

    [HttpGet("{id:guid}/stock")]
    [ProducesResponseType(typeof(ProductStockResponseDto), 200)]
    public async Task<IActionResult> GetProductStockById(Guid id)
    {
        var product = await mongoDbContext.Products
                                .Find(p => p.Id == id.ToString())
                                .FirstOrDefaultAsync();

        if (product == null)
            return NotFound(new { message = "Produto não encontrado." });

        return Ok(new ProductStockResponseDto
        {
            Product = new ProductResponseDto
            {
                Id = Guid.Parse(product.Id!),
                Name = product.Name!,
                Description = product.Description!,
                Price = product.Price!.Value
            },
            StockQuantity = product.Stock!.Value
        });
    }

    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<ProductResponseDto>), 200)]
    public async Task<IActionResult> GetProducts([FromQuery] int page = 1, [FromQuery] int size = 10)
    {
        page = page <= 0 ? 1 : page;
        size = size <= 0 || size > 10 ? 10 : size;

        var products = await mongoDbContext.Products
                        .Find(_ => true)
                        .Skip((page - 1) * size)
                        .Limit(size)
                        .ToListAsync();

        if (!products.Any())
            return NoContent();

        return Ok(products.Select(p => new ProductResponseDto
        {
            Id = Guid.Parse(p.Id!),
            Name = p.Name!,
            Description = p.Description!,
            Price = p.Price!.Value
        }));
    }
}