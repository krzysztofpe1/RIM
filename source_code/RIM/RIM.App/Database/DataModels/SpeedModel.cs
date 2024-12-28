using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace RIM.App.Database.DataModels;

public class SpeedModel
{

    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public required ObjectId Id { get; set; }

    public required int SensorId { get; set; }

    [BsonDateTimeOptions(Kind = DateTimeKind.Utc)]
    public required DateTime CreatedAt { get; set; }

    public required double SpeedValue { get; set; }

}