using System.Text.Json.Serialization;
using RIM.App.Utils;

namespace RIM.App.Mqtt.DataModels;

public class SensorData
{

    public required int SensorId { get; set; }

    [JsonConverter(typeof(SensorTypeConverter))]
    public required SensorType SensorType { get; set; }

    public required double Value { get; set; }

    [JsonConverter(typeof(TimestampToDateTimeConverter))]
    public required DateTime Timestamp { get; set; }

}