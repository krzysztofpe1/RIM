using System.Text.Json;
using Microsoft.Extensions.Options;
using MongoDB.Bson;
using MQTTnet.Client;
using MQTTnet;
using RIM.App.Configurations;
using RIM.App.Database.DataModels;
using RIM.App.Database.Repositories;
using RIM.App.Utils;
using RIM.App.Mqtt.DataModels;
using RIM.App.Services;

namespace RIM.App.Mqtt;

public class MqttHostedService(IOptions<MqttClientSettings> mqttClientSettings, ILogger<MqttHostedService> logger, IServiceScopeFactory serviceScopeFactory)
    : IHostedService
{
    #region Private Variables

    private readonly MqttClientSettings _mqttClientSettings = mqttClientSettings.Value;
    private IMqttClient? _mqttClient;

    #endregion

    #region Event Handlers

    private async Task HandleDisconnectedAsync(MqttClientDisconnectedEventArgs e)
    {
        logger.LogWarning($"Disconnected from MQTT broker. Reason: {e.Reason}");

        await Task.Delay(TimeSpan.FromSeconds(5));
        try
        {
            await _mqttClient.ReconnectAsync();
            logger.LogInformation("Reconnected to MQTT broker.");
        }
        catch (Exception ex)
        {
            logger.LogError($"Failed to reconnect to MQTT broker: {ex.Message}");
        }
    }

    private async Task HandleApplicationMessageReceivedAsync(MqttApplicationMessageReceivedEventArgs e)
    {
        var payload = System.Text.Encoding.UTF8.GetString(e.ApplicationMessage.Payload);
        logger.LogInformation($"Message received on topic {e.ApplicationMessage.Topic}: {payload}");

        try
        {
            var options = new JsonSerializerOptions
            {
                PropertyNamingPolicy = new SnakeCaseNamingPolicy(),
                Converters = { new TimestampToDateTimeConverter() },
                PropertyNameCaseInsensitive = true
            };

            var data = JsonSerializer.Deserialize<SensorData>(payload, options);
            if (data != null)
            {
                await HandleSensorData(data);
            }
        }
        catch (Exception ex)
        {
            logger.LogError($"Failed to deserialize message: {ex.Message}");
        }
    }

    private async Task HandleSensorData(SensorData data)
    {
        try
        {
            logger.LogInformation(
                $"Received Sensor Data: \"Sensor ID: {data.SensorId}, Type: {data.SensorType}, Value: {data.Value}, Timestamp: {data.Timestamp}\".");

            var serviceProvider = serviceScopeFactory.CreateScope().ServiceProvider;

            var repository = serviceProvider.GetRequiredService<SensorDataModelRepository>();
            var blockchainService = serviceProvider.GetRequiredService<BlockchainService>();

            var sensorDataModel = new SensorDataModel
            {
                SensorId = data.SensorId,
                SensorType = Enum.Parse<SensorTypeModel>(data.SensorType.ToString()),
                Value = data.Value,
                Timestamp = data.Timestamp
            };

            var destinationAccount = blockchainService.GetAllAccounts()[data.SensorId + 1];

            await blockchainService.TransferAsync(destinationAccount!, 10);

            repository.Insert(sensorDataModel);
        }
        catch (Exception ex)
        {
            logger.LogWarning("Failed to persist sensor data. Exception message: " + ex.Message);
        }
    }

    #endregion

    #region Subscription

    private async Task SubscribeToTopicAsync()
    {
        if (_mqttClient is { IsConnected: true })
        {
            try
            {
                await _mqttClient.SubscribeAsync(new MqttTopicFilterBuilder()
                    .WithTopic(_mqttClientSettings.Topic)
                    .Build());
                logger.LogInformation($"Subscribed to topic: {_mqttClientSettings.Topic}");
            }
            catch (Exception ex)
            {
                logger.LogError($"Failed to subscribe to topic {_mqttClientSettings.Topic}: {ex.Message}");
            }
        }
        else
        {
            logger.LogWarning("Cannot subscribe to topic because the client is not connected.");
        }
    }

    #endregion

    #region IHostedService Implementation

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        logger.LogInformation("Starting MQTT client...");

        var options = new MqttClientOptionsBuilder()
            .WithClientId("RIMuser")
            .WithTcpServer(_mqttClientSettings.BrokerAddress, _mqttClientSettings.BrokerPort)
            .WithCleanSession()
            .WithProtocolVersion(MQTTnet.Formatter.MqttProtocolVersion.V311)
            //.WithKeepAlivePeriod(TimeSpan.FromSeconds(60))
            .Build();

        _mqttClient = new MqttFactory().CreateMqttClient();

        _mqttClient.DisconnectedAsync += HandleDisconnectedAsync;
        _mqttClient.ApplicationMessageReceivedAsync += HandleApplicationMessageReceivedAsync;

        try
        {
            var result = await _mqttClient.ConnectAsync(options, cancellationToken);

            if (result.ResultCode == MqttClientConnectResultCode.Success)
            {
                logger.LogInformation("Successfully connected to MQTT broker.");
                await SubscribeToTopicAsync();
            }
            else
            {
                logger.LogError($"Failed to connect to MQTT broker. Result: {result.ResultCode}");
            }
        }
        catch (Exception ex)
        {
            logger.LogError($"Error while connecting to MQTT broker: {ex.Message}");
        }
    }

    public async Task StopAsync(CancellationToken cancellationToken)
    {
        if (_mqttClient is { IsConnected: true })
        {
            logger.LogInformation("Disconnecting MQTT client...");
            await _mqttClient.DisconnectAsync(new MqttClientDisconnectOptions()
            {
                Reason = MqttClientDisconnectOptionsReason.NormalDisconnection,
            }, cancellationToken);
        }
    }

    #endregion
}
