namespace ProductManagement.API.DTOs.Queries;

public class ProductStockResponseDto
{
    public ProductResponseDto? Product { get; set; }
    public int StockQuantity { get; set; }
}