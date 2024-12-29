using System.Text.Json.Serialization;

namespace RIM.App.ViewDataModels;

public class ViewDashboardSensor
{

    public required int SensorId { get; set; }

    [JsonConverter(typeof(JsonStringEnumConverter))]
    public required ViewSensorType SensorType { get; set; }

    public required double LatestValue { get; set; }

    public required double AverageLatest100Value { get; set; }

}