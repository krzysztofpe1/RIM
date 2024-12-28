using MongoDB.Driver;

namespace RIM.App.Database.Repositories;

public abstract class MongoDbRepositoryBase<T>(IMongoClient client, string databaseName, string collectionName)
    where T : class
{
    protected readonly IMongoCollection<T> Collection = client.GetDatabase(databaseName).GetCollection<T>(collectionName);
}