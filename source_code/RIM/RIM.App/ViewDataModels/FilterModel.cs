namespace RIM.App.ViewDataModels;

public class FilterModel
{
    
    public DateTime? FromDate { get; set; }

    public DateTime? ToDate { get; set; }

    public ViewSensorType? SensorType { get; set; }

    public int? SensorId { get; set; }

    public int? ResultsPerPage { get; set; }

    /// <summary>
    /// Starting from 0
    /// </summary>
    public int? Page { get; set; }

    public SortedBy? SortBy { get; set; }

    public bool? SortedDescending { get; set; }

}