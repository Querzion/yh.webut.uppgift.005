using Microsoft.AspNetCore.Mvc;

namespace Presentation.WebApp.Controllers;

public class ProjectsController : Controller
{
    [Route("projects")]
    public IActionResult Projects()
    {
        return View();
    }
}