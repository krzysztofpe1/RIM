using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace RIM.App.Pages;

public class IndexModel() : PageModel
{
    public IActionResult OnGet()
    {
        return Redirect("/sensors");
    }
}