using Bogus;
using ProductManagement.API.DTOs.Commands;

namespace ProductManagement.Infra.Tests.Fakers;

public sealed class CreateProductDtoFaker : Faker<CreateProductDto>
{
    public CreateProductDtoFaker()
    {
        RuleFor(p => p.Name, f => f.Commerce.ProductName());
        RuleFor(p => p.Description, f => f.Commerce.ProductDescription());
        RuleFor(p => p.Price, f => f.Finance.Amount(1, 9999));
        RuleFor(p => p.InitialStock, f => f.Random.Int(1, 500));
    }
}
