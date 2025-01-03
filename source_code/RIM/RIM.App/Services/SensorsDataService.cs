using System.Reflection;
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

    public List<ViewDashboardSensor> GetSensorCompactData()
    {
        var sensorsData = repository.GetAll();
        var sensorIds = sensorsData.Select(x => x.SensorId).Distinct();

        var results = new List<ViewDashboardSensor>();

        foreach (var sensorId in sensorIds)
        {

            var relevantSensorData = sensorsData.Where(x => x.SensorId == sensorId).OrderByDescending(x => x.Timestamp);
            if (!relevantSensorData.Any())
                continue;

            var first = relevantSensorData.First();

            var dashboardItem = new ViewDashboardSensor()
            {
                SensorId = sensorId,
                SensorType = Enum.Parse<ViewSensorType>(first.SensorType.ToString()),
                LatestValue = first.Value,
                AverageLatest100Value = relevantSensorData.Take(100).Average(x => x.Value),
            };

            results.Add(dashboardItem);
        }

        return results.OrderBy(x => x.SensorId).ToList();
    }

    public long GetPagesCountForPageSize(FilterModel filterModel)
    {
        var count = repository.GetCount(filterModel);
        if (filterModel is { ResultsPerPage: not null })
            return count / filterModel.ResultsPerPage.Value;
        return 1;
    }

    public List<(int, SensorTypeModel)> GetSensors()
    {
        return repository.GetSensors();
    }

}