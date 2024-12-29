using MongoDB.Driver;
using RIM.App.Database.DataModels;
using RIM.App.ViewDataModels;
using System.ComponentModel;
using System.Reflection;

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

    public static FilterDefinition<T> BuildFilter<T>(this FilterModel filterModel) where T : SensorDataModel
    {
        var builder = Builders<T>.Filter;
        var filters = new List<FilterDefinition<T>>();

        // FromDate and ToDate filters
        if (filterModel.FromDate.HasValue)
            filters.Add(builder.Gte(x => x.Timestamp, filterModel.FromDate.Value));
        if (filterModel.ToDate.HasValue)
            filters.Add(builder.Lte(x => x.Timestamp, filterModel.ToDate.Value));

        // SensorId filter
        if (filterModel.SensorId.HasValue)
            filters.Add(builder.Eq(x => x.SensorId, filterModel.SensorId.Value));

        // SensorType filter
        if (filterModel.SensorType.HasValue)
            filters.Add(builder.Eq(x=>x.SensorType, Enum.Parse<SensorTypeModel>(filterModel.SensorType.ToString())));

        // Combine filters
        return filters.Any() ? builder.And(filters) : builder.Empty;
    }

    public static SortDefinition<T> BuildSort<T>(this FilterModel filterModel) where T : SensorDataModel
    {
        var builder = Builders<T>.Sort;
        if (!filterModel.SortBy.HasValue)
            return builder.Ascending(SortedBy.Timestamp.ToString());

        return filterModel.SortedDescending.HasValue && filterModel.SortedDescending.Value
            ? builder.Descending(filterModel.SortBy.ToString())
            : builder.Ascending(filterModel.SortBy.ToString());
    }

    public static class EnumExtensions
    {
        public static string DisplayName(Enum value)
        {
            if (value == null)
            {
                return null;
            }
            FieldInfo field = value.GetType().GetField(value.ToString());
            DescriptionAttribute[] attributes = (DescriptionAttribute[])field
                .GetCustomAttributes(typeof(DescriptionAttribute), false);
            if (attributes.Length > 0)
            {
                return attributes[0].Description;
            }
            return value.ToString();
        }
    }

}