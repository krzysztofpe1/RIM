using RIM.App.Database.DataModels;
using RIM.App.Database.Repositories;
using RIM.App.Utils;
using RIM.App.ViewDataModels;

namespace RIM.App.Services;

public class SensorsDataService(SpeedRepository speedRepository, LightIntensityRepository lightIntensityRepository, SurfaceTemperatureRepository surfaceTemperatureRepository, VibrationsRepository vibrationsRepository)
{
    public List<ViewSensorData> GetAll()
    {
        List<ViewSensorData> results = new();
        results.AddRange(speedRepository.GetAll().Select(x => new ViewSensorData()
        {
            Id = x.Id,
            SensorId = x.SensorId,
            Value = x.Value,
            SensorType = ViewSensorType.Speed,
            CreatedAt = x.CreatedAt
        }));
        results.AddRange(lightIntensityRepository.GetAll().Select(x => new ViewSensorData()
        {
            Id = x.Id,
            SensorId = x.SensorId,
            Value = x.Value,
            SensorType = ViewSensorType.LightIntensity,
            CreatedAt = x.CreatedAt
        }));
        results.AddRange(surfaceTemperatureRepository.GetAll().Select(x => new ViewSensorData()
        {
            Id = x.Id,
            SensorId = x.SensorId,
            Value = x.Value,
            SensorType = ViewSensorType.SurfaceTemperature,
            CreatedAt = x.CreatedAt
        }));
        results.AddRange(vibrationsRepository.GetAll().Select(x => new ViewSensorData()
        {
            Id = x.Id,
            SensorId = x.SensorId,
            Value = x.Value,
            SensorType = ViewSensorType.Vibrations,
            CreatedAt = x.CreatedAt
        }));
        return results;
    }

    public List<ViewSensorData> GetFiltered(FilterModel filterModel)
    {
        // Define a function to convert BaseModel to ViewSensorData
        Func<BaseModel, ViewSensorData> conversionFunc = x => new ViewSensorData
        {
            Id = x.Id,
            SensorId = x.SensorId,
            Value = x.Value,
            SensorType = filterModel.SensorType ?? ViewSensorType.Speed, // Default or based on filter
            CreatedAt = x.CreatedAt
        };

        // Determine the page and results per page
        int pageSize = filterModel.ResultsPerPage ?? 10;
        int page = filterModel.Page ?? 0;

        // Filtered results for specific SensorType
        if (filterModel.SensorType.HasValue)
        {
            switch (filterModel.SensorType)
            {
                case ViewSensorType.Speed:
                    return speedRepository
                        .GetByFilter(filterModel.BuildFilter<SpeedModel>(), pageSize, page)
                        .Select(conversionFunc)
                        .ToList();
                case ViewSensorType.LightIntensity:
                    return lightIntensityRepository
                        .GetByFilter(filterModel.BuildFilter<LightIntensityModel>(), pageSize, page)
                        .Select(conversionFunc)
                        .ToList();
                case ViewSensorType.SurfaceTemperature:
                    return surfaceTemperatureRepository
                        .GetByFilter(filterModel.BuildFilter<SurfaceTemperatureModel>(), pageSize, page)
                        .Select(conversionFunc)
                        .ToList();
                case ViewSensorType.Vibrations:
                    return vibrationsRepository
                        .GetByFilter(filterModel.BuildFilter<VibrationsModel>(), pageSize, page)
                        .Select(conversionFunc)
                        .ToList();
            }
        }

        // Combine results from all repositories if no specific SensorType is provided
        var results = new List<ViewSensorData>();

        results.AddRange(speedRepository.GetByFilter(filterModel.BuildFilter<SpeedModel>()).Select(conversionFunc));
        results.AddRange(lightIntensityRepository.GetByFilter(filterModel.BuildFilter<LightIntensityModel>()).Select(conversionFunc));
        results.AddRange(surfaceTemperatureRepository.GetByFilter(filterModel.BuildFilter<SurfaceTemperatureModel>()).Select(conversionFunc));
        results.AddRange(vibrationsRepository.GetByFilter(filterModel.BuildFilter<VibrationsModel>()).Select(conversionFunc));

        if(filterModel is { ResultsPerPage: not null, Page: not null })
            results = results.GetRange(filterModel.ResultsPerPage.Value * filterModel.Page.Value - 1, filterModel.ResultsPerPage.Value);

        return results;
    }

    public long GetPagesCountForPageSize(FilterModel filterModel)
    {
        long count = 0;
        if (filterModel.SensorType.HasValue)
        {
            switch (filterModel.SensorType)
            {
                case ViewSensorType.Speed:
                    count = speedRepository.GetCount(filterModel.BuildFilter<SpeedModel>());
                    break;
                case ViewSensorType.LightIntensity:
                    count = lightIntensityRepository.GetCount(filterModel.BuildFilter<LightIntensityModel>());
                    break;
                case ViewSensorType.SurfaceTemperature:
                    count = surfaceTemperatureRepository.GetCount(filterModel.BuildFilter<SurfaceTemperatureModel>());
                    break;
                case ViewSensorType.Vibrations:
                    count = vibrationsRepository.GetCount(filterModel.BuildFilter<VibrationsModel>());
                    break;
            }
        }
        else
        {
            count = speedRepository.GetCount(filterModel.BuildFilter<SpeedModel>()) +
                    lightIntensityRepository.GetCount(filterModel.BuildFilter<LightIntensityModel>()) +
                    surfaceTemperatureRepository.GetCount(filterModel.BuildFilter<SurfaceTemperatureModel>()) +
                    vibrationsRepository.GetCount(filterModel.BuildFilter<VibrationsModel>());
        }

        if (filterModel.ResultsPerPage.HasValue)
            return (long)(count / filterModel.ResultsPerPage.Value);

        return count;
    }

}