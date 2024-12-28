using Microsoft.Extensions.Options;
using MongoDB.Driver;
using RIM.App.Database.DataModels;

namespace RIM.App.Database.Repositories;

public class VibrationsRepository(IMongoClient client, IOptions<RimDbSettings> dbSettings)
    : MongoDbRepositoryBase<VibrationsModel>(client, dbSettings.Value.DatabaseName, dbSettings.Value.VibrationsCollectionName)
{
    public List<VibrationsModel> GetAll() => Collection.Find(_ => true).ToList();
    public List<VibrationsModel> GetAllBySensorId(int id) => Collection.Find(x => x.SensorId == id).ToList();
    public void Insert(VibrationsModel model) => Collection.InsertOne(model);
}