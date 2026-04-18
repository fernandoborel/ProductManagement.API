using MediatR;
using ProductManagement.API.DTOs.Commands;
using ProductManagement.API.Notifications;
using ProductManagement.Infra.Data.SqlServer.Contexts;
using ProductManagement.Infra.Data.SqlServer.Entities;
using System.Diagnostics;
using System.Text.Json;

namespace ProductManagement.API.RequestHandlers;

public class ProductRequestHandler(IMediator mediator, SqlServerContext context) :
    IRequestHandler<CreateProductDto>,
    IRequestHandler<UpdateProductDto>,
    IRequestHandler<UpdateStockDto>
{
    public async Task Handle(CreateProductDto request, CancellationToken cancellationToken)
    {
        Debug.WriteLine("Gravando o produto no banco de dados SQL.");

        var product = new Product
        {
            Name = request.Name,
            Description = request.Description,
            Price = request.Price,
            Stock = request.InitialStock,
            CreatedAt = DateTime.Now
        };

        await context.Products.AddAsync(product);
        await context.SaveChangesAsync();

        var notification = new ProductNotification
        {
            Action = ActionNotification.CreateProduct,
            Data = JsonSerializer.Serialize(request),
            Id = product.Id
        };

        await mediator.Publish(notification);
    }

    public async Task Handle(UpdateProductDto request, CancellationToken cancellationToken)
    {
        Debug.WriteLine("Atualizando o produto no banco de dados SQL.");

        var product = await context.Products.FindAsync(request.Id);

        if (product == null)
            throw new ApplicationException("Produto não encontrado.");

        product.Name = request.Name;
        product.Price = request.Price;
        product.Description = request.Description;
        product.ModifiedAt = DateTime.Now;

        context.Products.Update(product);

        await context.SaveChangesAsync();

        var notification = new ProductNotification
        {
            Action = ActionNotification.UpdateProduct,
            Data = JsonSerializer.Serialize(request)
        };

        await mediator.Publish(notification);
    }

    public async Task Handle(UpdateStockDto request, CancellationToken cancellationToken)
    {
        Debug.WriteLine("Atualizando o estoque do produto no banco de dados SQL.");

        var product = await context.Products.FindAsync(request.Id);

        if (product == null)
            throw new ApplicationException("Produto não encontrado.");

        product.Stock = request.Quantity;
        product.ModifiedAt = DateTime.Now;

        context.Products.Update(product);

        await context.SaveChangesAsync();

        var notification = new ProductNotification
        {
            Action = ActionNotification.UpdateStock,
            Data = JsonSerializer.Serialize(request)
        };

        await mediator.Publish(notification);
    }
}