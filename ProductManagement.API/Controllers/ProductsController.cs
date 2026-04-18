using MediatR;
using Microsoft.AspNetCore.Mvc;
using ProductManagement.API.DTOs.Commands;
using ProductManagement.API.DTOs.Queries;

namespace ProductManagement.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class ProductsController(IMediator mediator) : ControllerBase
{
    [HttpPost]
    [ProducesResponseType(typeof(ProductResponseDto), 201)]
    public async Task<IActionResult> CreateProduct([FromBody] CreateProductDto dto)
    {
        await mediator.Send(dto);
        return Ok();
    }

    [HttpPut("{id:guid}")]
    [ProducesResponseType(typeof(ProductResponseDto), 200)]
    public async Task<IActionResult> UpdateProduct(Guid id, [FromBody] UpdateProductDto dto)
    {
        dto.Id = id;
        await mediator.Send(dto);
        return Ok();
    }

    [HttpPut("{id:guid}/stock")]
    [ProducesResponseType(typeof(ProductStockResponseDto), 200)]
    public async Task<IActionResult> UpdateStock(Guid id, [FromBody] UpdateStockDto dto)
    {
        dto.Id = id;
        await mediator.Send(dto);
        return Ok();
    }

    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(ProductResponseDto), 200)]
    public async Task<IActionResult> GetProductById(Guid id)
    {
        return Ok();
    }

    [HttpGet("{id:guid}/stock")]
    [ProducesResponseType(typeof(ProductStockResponseDto), 200)]
    public async Task<IActionResult> GetProductStockById(Guid id)
    {
        return Ok();
    }

    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<ProductResponseDto>), 200)]
    public async Task<IActionResult> GetProducts([FromQuery] int page = 1, [FromQuery] int pageSize = 10)
    {
        return Ok();
    }
}