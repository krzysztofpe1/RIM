using MongoDB.Bson;

namespace RIM.App.Database.DataModels;

public class SensorDataModel
{

    public ObjectId Id { get; set; } = ObjectId.Empty;

    public required int SensorId { get; set; }

    public required SensorTypeModel SensorType { get; set; }

    public required double Value { get; set; }

    public required DateTime Timestamp { get; set; }
}