using MongoDB.Bson;

namespace RIM.App.ViewDataModels;

public class ViewSensorData
{

    public required ObjectId Id;
    
    public required int SensorId { get; set; }

    public required ViewSensorType SensorType { get; set; }

    public required double Value { get; set; }

    public required DateTime CreatedAt { get; set; }

}