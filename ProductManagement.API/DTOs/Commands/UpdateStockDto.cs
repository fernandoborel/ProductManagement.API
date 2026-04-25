using MediatR;
using System.Text.Json.Serialization;

namespace ProductManagement.API.DTOs.Commands;

public class UpdateStockDto : IRequest<Guid>
{
    [JsonIgnore]
    public Guid Id { get; set; }
    public int Quantity { get; set; }
}