using MediatR;
using Microsoft.EntityFrameworkCore;
using Moq;
using ProductManagement.API.Notifications;
using ProductManagement.API.RequestHandlers;
using ProductManagement.Infra.Data.SqlServer.Contexts;
using ProductManagement.Infra.Data.SqlServer.Entities;
using ProductManagement.Infra.Tests.Fakers;

namespace ProductManagement.Infra.Tests.RequestHandlers;

public class ProductRequestHandlerTests : IDisposable
{
    private readonly SqlServerContext _context;
    private readonly Mock<IMediator> _mediatorMock;
    private readonly ProductRequestHandler _handler;

    public ProductRequestHandlerTests()
    {
        var options = new DbContextOptionsBuilder<SqlServerContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        _context = new SqlServerContext(options);
        _mediatorMock = new Mock<IMediator>();
        _handler = new ProductRequestHandler(_mediatorMock.Object, _context);
    }

    // ─── CreateProduct ────────────────────────────────────────────────────────

    [Fact]
    public async Task Handle_CreateProduct_ShouldPersistProductAndReturnId()
    {
        var dto = new CreateProductDtoFaker().Generate();

        var id = await _handler.Handle(dto, CancellationToken.None);

        var saved = await _context.Products.FindAsync(id);
        Assert.NotEqual(Guid.Empty, id);
        Assert.NotNull(saved);
        Assert.Equal(dto.Name, saved.Name);
        Assert.Equal(dto.Description, saved.Description);
        Assert.Equal(dto.Price, saved.Price);
        Assert.Equal(dto.InitialStock, saved.Stock);
    }

    [Fact]
    public async Task Handle_CreateProduct_ShouldPublishNotification()
    {
        var dto = new CreateProductDtoFaker().Generate();

        await _handler.Handle(dto, CancellationToken.None);

        _mediatorMock.Verify(m =>
            m.Publish(
                It.Is<ProductNotification>(n => n.Action == ActionNotification.CreateProduct),
                It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task Handle_CreateProduct_ShouldSetCreatedAtToNow()
    {
        var dto = new CreateProductDtoFaker().Generate();
        var before = DateTime.Now;

        var id = await _handler.Handle(dto, CancellationToken.None);

        var saved = await _context.Products.FindAsync(id);
        Assert.NotNull(saved);
        Assert.True(saved.CreatedAt >= before);
    }

    // ─── UpdateProduct ────────────────────────────────────────────────────────

    [Fact]
    public async Task Handle_UpdateProduct_ShouldUpdateFieldsAndReturnId()
    {
        var product = CreateAndSaveProduct();
        var dto = new UpdateProductDtoFaker().Generate();
        dto.Id = product.Id;

        var id = await _handler.Handle(dto, CancellationToken.None);

        var updated = await _context.Products.FindAsync(id);
        Assert.Equal(product.Id, id);
        Assert.NotNull(updated);
        Assert.Equal(dto.Name, updated.Name);
        Assert.Equal(dto.Description, updated.Description);
        Assert.Equal(dto.Price, updated.Price);
    }

    [Fact]
    public async Task Handle_UpdateProduct_ShouldPublishNotification()
    {
        var product = CreateAndSaveProduct();
        var dto = new UpdateProductDtoFaker().Generate();
        dto.Id = product.Id;

        await _handler.Handle(dto, CancellationToken.None);

        _mediatorMock.Verify(m =>
            m.Publish(
                It.Is<ProductNotification>(n => n.Action == ActionNotification.UpdateProduct && n.Id == product.Id),
                It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task Handle_UpdateProduct_ShouldThrowWhenProductNotFound()
    {
        var dto = new UpdateProductDtoFaker().Generate();
        dto.Id = Guid.NewGuid();

        await Assert.ThrowsAsync<ApplicationException>(() =>
            _handler.Handle(dto, CancellationToken.None));
    }

    [Fact]
    public async Task Handle_UpdateProduct_ShouldSetModifiedAt()
    {
        var product = CreateAndSaveProduct();
        var dto = new UpdateProductDtoFaker().Generate();
        dto.Id = product.Id;
        var before = DateTime.Now;

        var id = await _handler.Handle(dto, CancellationToken.None);

        var updated = await _context.Products.FindAsync(id);
        Assert.NotNull(updated);
        Assert.True(updated.ModifiedAt >= before);
    }

    // ─── UpdateStock ──────────────────────────────────────────────────────────

    [Fact]
    public async Task Handle_UpdateStock_ShouldUpdateStockAndReturnId()
    {
        var product = CreateAndSaveProduct();
        var dto = new UpdateStockDtoFaker().Generate();
        dto.Id = product.Id;

        var id = await _handler.Handle(dto, CancellationToken.None);

        var updated = await _context.Products.FindAsync(id);
        Assert.Equal(product.Id, id);
        Assert.NotNull(updated);
        Assert.Equal(dto.Quantity, updated.Stock);
    }

    [Fact]
    public async Task Handle_UpdateStock_ShouldPublishNotification()
    {
        var product = CreateAndSaveProduct();
        var dto = new UpdateStockDtoFaker().Generate();
        dto.Id = product.Id;

        await _handler.Handle(dto, CancellationToken.None);

        _mediatorMock.Verify(m =>
            m.Publish(
                It.Is<ProductNotification>(n => n.Action == ActionNotification.UpdateStock && n.Id == product.Id),
                It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task Handle_UpdateStock_ShouldThrowWhenProductNotFound()
    {
        var dto = new UpdateStockDtoFaker().Generate();
        dto.Id = Guid.NewGuid();

        await Assert.ThrowsAsync<ApplicationException>(() =>
            _handler.Handle(dto, CancellationToken.None));
    }

    [Fact]
    public async Task Handle_UpdateStock_ShouldSetModifiedAt()
    {
        var product = CreateAndSaveProduct();
        var dto = new UpdateStockDtoFaker().Generate();
        dto.Id = product.Id;
        var before = DateTime.Now;

        var id = await _handler.Handle(dto, CancellationToken.None);

        var updated = await _context.Products.FindAsync(id);
        Assert.NotNull(updated);
        Assert.True(updated.ModifiedAt >= before);
    }

    // ─── Helpers ──────────────────────────────────────────────────────────────

    private Product CreateAndSaveProduct()
    {
        var product = new Product
        {
            Name = "Product Test",
            Description = "Description Test",
            Price = 99.99m,
            Stock = 10,
            CreatedAt = DateTime.Now
        };

        _context.Products.Add(product);
        _context.SaveChanges();
        _context.ChangeTracker.Clear();

        return product;
    }

    public void Dispose() => _context.Dispose();
}
