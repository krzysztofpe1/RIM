using MongoDB.Driver;
using RIM.App.Database.DataModels;
using RIM.App.ViewDataModels;

namespace RIM.App.Utils;

public static class DateTimeExtensions
{

    public static bool Between(this DateTime toCompare, DateTime? dateFrom, DateTime? dateTo)
    {
        dateFrom ??= DateTime.MinValue;
        dateTo ??= DateTime.MaxValue;

        return toCompare.CompareTo(dateFrom) >= 0 && toCompare.CompareTo(dateTo) <= 0;
    }

}

public static class FilterModelExtensions
{

    public static FilterDefinition<T> BuildFilter<T>(this FilterModel filterModel) where T : BaseModel
    {
        var builder = Builders<T>.Filter;
        var filters = new List<FilterDefinition<T>>();

        // Date range filter
        if (filterModel is { FromDate: not null, ToDate: not null })
        {
            filters.Add(builder.And(
                builder.Gte(x => x.CreatedAt, filterModel.FromDate.Value),
                builder.Lte(x => x.CreatedAt, filterModel.ToDate.Value)
            ));
        }
        else if (filterModel.FromDate.HasValue)
        {
            filters.Add(builder.Gte(x => x.CreatedAt, filterModel.FromDate.Value));
        }
        else if (filterModel.ToDate.HasValue)
        {
            filters.Add(builder.Lte(x => x.CreatedAt, filterModel.ToDate.Value));
        }

        // SensorId filter
        if (filterModel.SensorId.HasValue)
        {
            filters.Add(builder.Eq(x => x.SensorId, filterModel.SensorId.Value));
        }

        // Combine filters
        return filters.Any() ? builder.And(filters) : builder.Empty;
    }

}