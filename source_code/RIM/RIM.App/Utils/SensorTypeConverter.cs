using RIM.App.Mqtt.DataModels;
using System.Text.Json.Serialization;
using System.Text.Json;

namespace RIM.App.Utils;

public class SensorTypeConverter : JsonConverter<SensorType>
{
    public override SensorType Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        var value = reader.GetString();
        if (string.IsNullOrEmpty(value))
        {
            throw new JsonException("Invalid sensor type.");
        }

        // conversion from lowercase to PascalCase
        if (Enum.TryParse<SensorType>(char.ToUpper(value[0]) + value.Substring(1), out var sensorType))
        {
            return sensorType;
        }

        throw new JsonException($"Unknown sensor type: {value}");
    }

    public override void Write(Utf8JsonWriter writer, SensorType value, JsonSerializerOptions options)
    {
        // parsing the enum value to lowercase
        writer.WriteStringValue(value.ToString().ToLower());
    }
}