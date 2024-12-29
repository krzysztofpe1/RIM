using Microsoft.Extensions.Options;
using MongoDB.Bson;
using MongoDB.Driver;
using RIM.App.Configurations;
using RIM.App.Database.DataModels;

namespace RIM.App.Database.Repositories;

public class SpeedRepository(IMongoClient mongoClient, IOptions<RimDbSettings> dbSettings)
    : MongoDbRepositoryBase<SpeedModel>(mongoClient, dbSettings.Value.DatabaseName, dbSettings.Value.SpeedCollectionName)
{
    public List<SpeedModel> GetAll() => Collection.Find(_ => true).ToList();
    public List<SpeedModel> GetAllBySensorId(ObjectId id) => Collection.Find(x => x.Id == id).ToList();
    public void Insert(SpeedModel speedModel) => Collection.InsertOne(speedModel);
}