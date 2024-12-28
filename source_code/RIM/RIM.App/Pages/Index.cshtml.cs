using Microsoft.AspNetCore.Mvc.RazorPages;
using MongoDB.Bson;
using RIM.App.Database.DataModels;
using RIM.App.Database.Repositories;

namespace RIM.App.Pages;

public class IndexModel(SpeedSensorsRepository repository) : PageModel
{
    public void OnGet()
    {
        
    }
}