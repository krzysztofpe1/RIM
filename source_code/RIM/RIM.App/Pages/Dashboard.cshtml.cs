using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using RIM.App.Services;
using RIM.App.ViewDataModels;

namespace RIM.App.Pages;

[BindProperties]
public class DashboardModel(SensorsDataService service) : PageModel
{
    public List<ViewDashboardSensor> Results { get; set; }

    public void OnGet()
    {
        Results = service.GetSensorCompactData();
    }

    public IActionResult OnGetRefresh()
    {
        Results = service.GetSensorCompactData();
        return new JsonResult(Results);
    }

}