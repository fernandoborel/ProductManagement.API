using MongoDB.Bson.Serialization.Attributes;

namespace ProductManagement.Infra.Data.MongoDB.Documents;

public class ProductDocument
{
    [BsonId]
    public string? Id { get; set; }
    public string? Name { get; set; }
    public string? Description { get; set; }
    public decimal? Price { get; set; }
    public int? Stock { get; set; }
}