using System.ComponentModel;

namespace RIM.App.ViewDataModels;

public enum ViewSensorType
{
    Speed,
    [Description("Light Intensity")]
    Light,
    [Description("Surface Temperature")]
    Temperature,
    Vibration
}