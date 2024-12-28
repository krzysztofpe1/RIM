namespace RIM.App.Database;

public class RimDbSettings
{
    
    public string ConnectionString { get; set; } = null!;

    public string DatabaseName { get; set; } = null!;

    public string SpeedCollectionName { get; set; } = null!;

    public string LightIntensityCollectionName { get; set; } = null!;

    public string SurfaceTemperatureCollectionName { get; set; } = null!;

    public string VibrationsCollectionName { get; set; } = null!;

}