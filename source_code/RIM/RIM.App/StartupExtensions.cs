using Microsoft.Extensions.Options;
using MongoDB.Driver;
using RIM.App.Configurations;
using RIM.App.Database.Repositories;
using RIM.App.Mqtt;

namespace RIM.App;

public static class StartupExtensions
{

    public static void ConfigureServices(this IServiceCollection services)
    {
        services.ConfigureMongoDbClient();
        services.AddRepositories();
        services.AddMqtt();
    }

    public static IConfiguration GetConfiguration(this IServiceCollection services)
    {
        return services.BuildServiceProvider().GetRequiredService<IConfiguration>();
    }

    private static void ConfigureMongoDbClient(this IServiceCollection services)
    {
        var configuration = services.GetConfiguration();
        // Adding Mongo Db Context
        services.Configure<RimDbSettings>(configuration.GetSection("MongoDb"));

        var mongoDbSettings = services.BuildServiceProvider().GetRequiredService<IOptions<RimDbSettings>>();

        var settings = MongoClientSettings.FromConnectionString(mongoDbSettings.Value.ConnectionString);

        //settings.SslSettings = new SslSettings() { EnabledSslProtocols = SslProtocols.Tls12 };

        services.AddSingleton<IMongoClient>(new MongoClient(settings));
    }

    private static void AddRepositories(this IServiceCollection services)
    {
        services.AddScoped<SpeedRepository>();
        services.AddScoped<LightIntensityRepository>();
        services.AddScoped<SurfaceTemperatureRepository>();
        services.AddScoped<VibrationsRepository>();
    }

    private static void AddMqtt(this IServiceCollection services)
    {
        var configuration = services.GetConfiguration();
        services.Configure<MqttClientSettings>(configuration.GetSection("MQTT"));

        services.AddHostedService<MqttHostedService>();
    }

}