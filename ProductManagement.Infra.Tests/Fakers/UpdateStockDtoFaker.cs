using Bogus;
using ProductManagement.API.DTOs.Commands;

namespace ProductManagement.Infra.Tests.Fakers;

public sealed class UpdateStockDtoFaker : Faker<UpdateStockDto>
{
    public UpdateStockDtoFaker()
    {
        RuleFor(p => p.Id, f => f.Random.Guid());
        RuleFor(p => p.Quantity, f => f.Random.Int(0, 1000));
    }
}
