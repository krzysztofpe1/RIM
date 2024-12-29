using Microsoft.Extensions.Options;
using MongoDB.Driver;
using RIM.App.Configurations;
using RIM.App.Database.DataModels;

namespace RIM.App.Database.Repositories;

public class LightIntensityRepository(IMongoClient client, IOptions<RimDbSettings> dbSettings)
    : MongoDbRepositoryBase<LightIntensityModel>(client, dbSettings.Value.DatabaseName, dbSettings.Value.LightIntensityCollectionName)
{
    
    public List<LightIntensityModel> GetAllBySensorId(int id) => Collection.Find(x => x.SensorId == id).ToList();

}