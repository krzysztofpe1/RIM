using System.Numerics;

namespace RIM.App.ViewDataModels;

public class SensorWithBalance
{
 
    public required int SensorId { get; set; }

    public required ViewSensorType SensorType { get; set; }

    public required string WalletAddress { get; set; } 

    public required BigInteger Balance { get; set; }

}