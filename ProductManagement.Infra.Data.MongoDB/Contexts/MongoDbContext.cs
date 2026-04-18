using MongoDB.Driver;
using ProductManagement.Infra.Data.MongoDB.Documents;

namespace ProductManagement.Infra.Data.MongoDB.Contexts;

public class MongoDbContext
{
    private readonly IMongoDatabase mongoDatabase;

    public MongoDbContext(string connectionString, string databaseName)
    {
        var client = new MongoClient(connectionString);

        mongoDatabase = client.GetDatabase(databaseName);
    }

    public IMongoCollection<ProductDocument> Products
    {
        get
        {
            return mongoDatabase.GetCollection<ProductDocument>("products");
        }
    }
}