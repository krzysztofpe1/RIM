using System.Text.Json;
using System.Text.Json.Serialization;

namespace RIM.App.Utils;

public class TimestampToDateTimeConverter : JsonConverter<DateTime>
{
    public override DateTime Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        // conversion from double to DateTime
        var timestamp = reader.GetDouble();
        return DateTimeOffset.FromUnixTimeSeconds((long)timestamp).UtcDateTime;
    }

    public override void Write(Utf8JsonWriter writer, DateTime value, JsonSerializerOptions options)
    {
        // conversion from DateTime to double
        var unixTimestamp = new DateTimeOffset(value).ToUnixTimeSeconds();
        writer.WriteNumberValue(unixTimestamp);
    }
}