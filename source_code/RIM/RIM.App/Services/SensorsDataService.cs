using RIM.App.Database.DataModels;
using RIM.App.Database.Repositories;
using RIM.App.ViewDataModels;

namespace RIM.App.Services;

public class SensorsDataService(SensorDataModelRepository repository)
{
    public List<ViewSensorData> GetAll()
    {
        return repository.GetAll().Select(x => new ViewSensorData
        {
            Id = x.Id,
            SensorId = x.SensorId,
            Value = x.Value,
            SensorType = Enum.Parse<ViewSensorType>(x.SensorType.ToString()),
            CreatedAt = x.Timestamp
        }).ToList();
    }

    public List<ViewSensorData> GetFiltered(FilterModel filterModel)
    {
        // Define a function to convert BaseModel to ViewSensorData
        ViewSensorData ConversionFunc(SensorDataModel x) =>
            new ViewSensorData()
            {
                Id = x.Id,
                SensorId = x.SensorId,
                Value = x.Value,
                SensorType = Enum.Parse<ViewSensorType>(x.SensorType.ToString()),
                CreatedAt = x.Timestamp
            };

        return repository.GetByFilter(filterModel).Select(ConversionFunc).ToList();
    }

    public long GetPagesCountForPageSize(FilterModel filterModel)
    {
        var count = repository.GetCount(filterModel);
        if(filterModel is {ResultsPerPage: not null})
            return count/ filterModel.ResultsPerPage.Value;
        return 1;
    }

}