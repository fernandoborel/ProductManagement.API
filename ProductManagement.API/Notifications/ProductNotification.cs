using MediatR;

namespace ProductManagement.API.Notifications;

public class ProductNotification : INotification
{
    public Guid Id { get; set; }
    public string Data { get; set; } = string.Empty;
    public ActionNotification Action { get; set; }
}

public enum ActionNotification
{
    CreateProduct = 1,
    UpdateProduct = 2,
    UpdateStock = 3
}