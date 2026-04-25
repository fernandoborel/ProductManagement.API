using MediatR;
using System.Text.Json.Serialization;

namespace ProductManagement.API.DTOs.Commands;

public class UpdateProductDto : IRequest<Guid>
{
    [JsonIgnore]
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public decimal Price { get; set; }
}