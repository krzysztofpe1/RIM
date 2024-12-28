﻿using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;

namespace RIM.App.Database.DataModels;

public class VibrationsModel
{

    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public required ObjectId Id { get; set; }

    public required int SensorId { get; set; }

    [BsonDateTimeOptions(Kind = DateTimeKind.Utc)]
    public required DateTime CreatedAt { get; set; }

    public required double VibrationsValue { get; set; }

}