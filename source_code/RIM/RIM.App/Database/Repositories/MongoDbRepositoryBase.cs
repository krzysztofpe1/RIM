using System.Collections.ObjectModel;
using System.Linq.Expressions;
using MongoDB.Driver;
using RIM.App.Database.DataModels;

namespace RIM.App.Database.Repositories;

public abstract class MongoDbRepositoryBase<T>(IMongoClient client, string databaseName, string collectionName)
    where T : BaseModel
{
    protected readonly IMongoCollection<T> Collection = client.GetDatabase(databaseName).GetCollection<T>(collectionName);

    public List<T> GetAll() => Collection.Find(_ => true).ToList();

    public void Insert(T model) => Collection.InsertOne(model);

    public List<T> GetByFilter(FilterDefinition<T> filter, int? pageSize = null, int? page = null)
    {
        return Collection.Find(filter)
            .Skip(page * pageSize)
            .Limit(pageSize)
            .ToList();
    }

    public long GetCount(FilterDefinition<T> filter)
    {
        return Collection.CountDocuments(filter);
    }

}