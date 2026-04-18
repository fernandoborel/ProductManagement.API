using MediatR;
using ProductManagement.API.DTOs.Commands;
using ProductManagement.Infra.Data.MongoDB.Contexts;
using ProductManagement.Infra.Data.MongoDB.Documents;
using System.Diagnostics;
using System.Text.Json;

namespace ProductManagement.API.Notifications;

public class ProductNotificationHandler(MongoDbContext context) : INotificationHandler<ProductNotification>
{
    public async Task Handle(ProductNotification notification, CancellationToken cancellationToken)
    {
        switch (notification.Action)
        {
            case ActionNotification.CreateProduct:
                Debug.WriteLine("Cadastrando produto no MongoDB");

                var createProduct = JsonSerializer.Deserialize<CreateProductDto>(notification.Data);

                await context.Products.InsertOneAsync(new ProductDocument
                {
                    Id = notification.Id.ToString(),
                    Name = createProduct!.Name,
                    Description = createProduct!.Description,
                    Price = createProduct!.Price,
                    Stock = createProduct!.InitialStock
                });

                break;

            case ActionNotification.UpdateProduct:
                Debug.WriteLine("Atualizando produto no MongoDB");
                break;

            case ActionNotification.UpdateStock:
                Debug.WriteLine("Atualizando estoque do produto no MongoDB");
                break;
        }
    }
}