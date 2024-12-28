using Microsoft.Extensions.Options;
using MongoDB.Driver;
using RIM.App.Database.DataModels;

namespace RIM.App.Database.Repositories;

public class SurfaceTemperatureRepository(IMongoClient client, IOptions<RimDbSettings> dbSettings)
    : MongoDbRepositoryBase<SurfaceTemperatureModel>(client, dbSettings.Value.DatabaseName, dbSettings.Value.SurfaceTemperatureCollectionName)
{
    public List<SurfaceTemperatureModel> GetAll() => Collection.Find(_ => true).ToList();
    public List<SurfaceTemperatureModel> GetAllBySensorId(int id) => Collection.Find(x => x.SensorId == id).ToList();
    public void Insert(SurfaceTemperatureModel model) => Collection.InsertOne(model);
}