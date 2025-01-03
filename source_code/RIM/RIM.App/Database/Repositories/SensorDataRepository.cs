using Microsoft.Extensions.Options;
using MongoDB.Driver;
using RIM.App.Configurations;
using RIM.App.Database.DataModels;
using RIM.App.Utils;
using RIM.App.ViewDataModels;

namespace RIM.App.Database.Repositories;

public class SensorDataModelRepository(IMongoClient client, IOptions<RimDbSettings> dbSettings)
{
    private readonly IMongoCollection<SensorDataModel> _collection = client.GetDatabase(dbSettings.Value.DatabaseName).GetCollection<SensorDataModel>(dbSettings.Value.CombinedCollectionName);

    public List<SensorDataModel> GetAll() => _collection.Find(_ => true).ToList();

    public void Insert(SensorDataModel model) => _collection.InsertOne(model);

    public List<SensorDataModel> GetByFilter(FilterModel filterModel)
    {
        // if no sorting is set, then sort by date descending
        if (!filterModel.SortBy.HasValue)
        {
            filterModel.SortBy = SortedBy.Timestamp;
            filterModel.SortedDescending = true;
        }

        var filter = filterModel.BuildFilter<SensorDataModel>();
        var sort = filterModel.BuildSort<SensorDataModel>();
        
        return _collection.Find(filter)
            .Sort(sort)
            .Skip(filterModel.Page * filterModel.ResultsPerPage)
            .Limit(filterModel.ResultsPerPage)
            .ToList();
    }

    public long GetCount(FilterModel filterModel)
    {
        var filter = filterModel.BuildFilter<SensorDataModel>();
        return _collection.CountDocuments(filter);
    }

    public List<(int, SensorTypeModel)> GetSensors()
    {
        var ids = _collection.AsQueryable().Select(x =>  x.SensorId).Distinct().ToList();

        return ids.Select(id => (id, _collection.AsQueryable().First(x => x.SensorId == id).SensorType)).ToList();
    }
}