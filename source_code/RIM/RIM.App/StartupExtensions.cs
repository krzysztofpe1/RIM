using Microsoft.Extensions.Options;
using MongoDB.Driver;
using RIM.App.Database;
using RIM.App.Database.Repositories;

namespace RIM.App;

public static class StartupExtensions
{

    public static void ConfigureServices(this IServiceCollection services)
    {
        services.ConfigureMongoDbClient();
        services.AddRepositories();
        services.AddMqtt();
    }

    private static void ConfigureMongoDbClient(this IServiceCollection services)
    {
        var configuration = services.BuildServiceProvider().GetRequiredService<IConfiguration>();
        // Adding Mongo Db Context
        services.Configure<RimDbSettings>(configuration.GetSection("MongoDb"));

        var mongoDbSettings = services.BuildServiceProvider().GetRequiredService<IOptions<RimDbSettings>>();

        var settings = MongoClientSettings.FromConnectionString(mongoDbSettings.Value.ConnectionString);

        //settings.SslSettings = new SslSettings() { EnabledSslProtocols = SslProtocols.Tls12 };

        services.AddSingleton<IMongoClient>(new MongoClient(settings));
    }

    private static void AddRepositories(this IServiceCollection services)
    {
        services.AddScoped<SpeedSensorsRepository>();
        services.AddScoped<LightIntensityRepository>();
        services.AddScoped<SurfaceTemperatureRepository>();
        services.AddScoped<VibrationsRepository>();
    }

    private static void AddMqtt(this IServiceCollection services)
    {

    }

}