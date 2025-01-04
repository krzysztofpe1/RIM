using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using RIM.App.Services;
using RIM.App.ViewDataModels;
using System.Text;
using System.Text.Json;

namespace RIM.App.Pages;

[BindProperties]
public class SensorsModel(SensorsDataService service) : PageModel
{

    public List<ViewSensorData> Results { get; set; } = [];

    public FilterModel Filter { get; set; } = new();

    private void FilterResults()
    {
        Results = service.GetFiltered(Filter);
    }

    public void OnGet(int page)
    {
        Filter.Page = page;
        Filter.ResultsPerPage ??= 30;

        FilterResults();
    }

    public void OnPost(int page)
    {
        FilterResults();
    }

    public IActionResult OnPostDownloadCsv()
    {
        FilterResults();

        // Generate CSV content in memory
        var csvBuilder = new StringBuilder();
        csvBuilder.AppendLine("Id;Sensor Id;Sensor Type;Value;CreatedAt"); // Header row

        foreach (var item in Results)
        {
            csvBuilder.AppendLine($"{item.Id};{item.SensorId};{item.SensorType};{item.Value};{item.CreatedAt}");
        }

        var csvBytes = Encoding.UTF8.GetBytes(csvBuilder.ToString());

        // Return the CSV as a file download
        return File(csvBytes, "text/csv", "data.csv");
    }

    public IActionResult OnPostDownloadJson()
    {
        FilterResults();

        var jsonString = JsonSerializer.Serialize(Results);

        var jsonBytes = Encoding.UTF8.GetBytes(jsonString);

        return File(jsonBytes, "application/json", "data.json");
    }

}