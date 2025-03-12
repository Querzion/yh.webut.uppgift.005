using Microsoft.AspNetCore.Mvc;

namespace Presentation.WebApp.Controllers;

[Route("admin")]
public class AdminController : Controller
{
    [Route("members")]
    public IActionResult Members()
    {
        return View();
    }
}