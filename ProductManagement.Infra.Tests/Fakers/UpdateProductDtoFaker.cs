using Bogus;
using ProductManagement.API.DTOs.Commands;

namespace ProductManagement.Infra.Tests.Fakers;

public sealed class UpdateProductDtoFaker : Faker<UpdateProductDto>
{
    public UpdateProductDtoFaker()
    {
        RuleFor(p => p.Id, f => f.Random.Guid());
        RuleFor(p => p.Name, f => f.Commerce.ProductName());
        RuleFor(p => p.Description, f => f.Commerce.ProductDescription());
        RuleFor(p => p.Price, f => f.Finance.Amount(1, 9999));
    }
}
