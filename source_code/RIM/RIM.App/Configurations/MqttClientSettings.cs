namespace RIM.App.Configurations;

public class MqttClientSettings
{
    public required string BrokerAddress { get; set; }
    public required int BrokerPort { get; set; }
    public required string Topic { get; set; }
    public required string ClientId { get; set; }
}